using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Domain.Entities.Identity;
using Shala.Domain.Entities.Organization;
using Shala.Domain.Entities.Platform;
using Shala.Shared.Requests.Tenant;
using Shala.Shared.Responses.Platform;

namespace Shala.Application.Features.Identity;

public class UserService : IUserService
{
    private static readonly HashSet<string> AllowedTenantRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "SchoolAdmin",
        "BranchAdmin",
        "Teacher",
        "Accountant",
        "Staff"
    };

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepository<Branch> _branchRepository;
    private readonly IGenericRepository<UserBranchAccess> _userBranchAccessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IGenericRepository<Branch> branchRepository,
        IGenericRepository<UserBranchAccess> userBranchAccessRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _branchRepository = branchRepository;
        _userBranchAccessRepository = userBranchAccessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, object Data)> GetUsersAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0)
            return Fail("Invalid tenant context.");

        var users = await _userManager.Users
            .Where(x => x.TenantId == tenantId)
            .Include(x => x.Branch)
            .Include(x => x.BranchAccesses)
                .ThenInclude(x => x.Branch)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new List<UserListItemResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var activeAccesses = user.BranchAccesses
                .Where(x => x.IsActive)
                .ToList();

            var hasAllBranchesAccess = activeAccesses.Any(x => x.HasAllBranchesAccess);

            result.Add(new UserListItemResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                MobileNumber = user.PhoneNumber ?? string.Empty,
                TenantId = user.TenantId,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                DefaultBranchId = user.BranchId,
                DefaultBranchName = user.Branch?.Name,
                HasAllBranchesAccess = hasAllBranchesAccess,
                AllowedBranchIds = hasAllBranchesAccess
                    ? new List<int>()
                    : activeAccesses
                    .Where(x => x.BranchId.HasValue)
                    .Select(x => x.BranchId!.Value)
                    .Distinct()
                    .ToList(),
                AllowedBranchNames = hasAllBranchesAccess
                    ? new List<string> { "All Branches" }
                    : activeAccesses
                        .Where(x => x.Branch != null)
                        .Select(x => x.Branch!.Name)
                        .Distinct()
                        .ToList()
            });
        }

        return (true, result);
    }

    public async Task<(bool Success, object Data)> CreateUserAsync(
        int tenantId,
        CreateTenantUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0)
            return Fail("Invalid tenant context.");

        if (request is null)
            return Fail("Request is required.");

        var validationError = ValidateCreateUserRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
            return Fail(validationError);

        var normalizedEmail = request.Email.Trim();
        var normalizedFullName = request.FullName.Trim();
        var normalizedPhone = string.IsNullOrWhiteSpace(request.MobileNumber)
            ? null
            : request.MobileNumber.Trim();

        var roleName = request.Role.ToString().Trim();

        if (!AllowedTenantRoles.Contains(roleName))
            return Fail("Selected role is not allowed.");

        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser is not null)
            return Fail("Email already exists.");

        var isSchoolAdmin = IsSchoolAdmin(roleName);

        var branchValidation = await BuildValidBranchAccessAsync(
            tenantId,
            request.DefaultBranchId,
            request.AllowedBranchIds,
            request.HasAllBranchesAccess || isSchoolAdmin,
            cancellationToken);

        if (!branchValidation.Success)
            return Fail(branchValidation.Message);

        var user = new ApplicationUser
        {
            FullName = normalizedFullName,
            UserName = normalizedEmail,
            Email = normalizedEmail,
            PhoneNumber = normalizedPhone,
            TenantId = tenantId,
            BranchId = request.DefaultBranchId,
            IsActive = true,
            EmailConfirmed = true
        };

        var transactionStarted = false;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            transactionStarted = true;

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                await SafeRollbackAsync(transactionStarted, cancellationToken);
                return Fail("User creation failed.", createResult.Errors.Select(x => x.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                await SafeRollbackAsync(transactionStarted, cancellationToken);
                return Fail("Role assignment failed.", roleResult.Errors.Select(x => x.Description));
            }

            await AddBranchAccessesAsync(
                tenantId,
                user.Id,
                request.DefaultBranchId,
                branchValidation.BranchIds,
                request.HasAllBranchesAccess || isSchoolAdmin,
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return (true, new
            {
                message = "User created successfully.",
                userId = user.Id,
                email = user.Email,
                defaultBranchId = user.BranchId,
                role = roleName
            });
        }
        catch (Exception ex)
        {
            if (transactionStarted)
                await SafeRollbackAsync(true, cancellationToken);

            var createdUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (createdUser is not null)
            {
                try
                {
                    await _userManager.DeleteAsync(createdUser);
                }
                catch
                {
                    // ignore cleanup error
                }
            }

            return Fail("An unexpected error occurred while creating the user.", new[] { ex.Message });
        }
    }

    public async Task<(bool Success, object Data)> UpdateUserAsync(
        int tenantId,
        string userId,
        UpdateTenantUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0)
            return Fail("Invalid tenant context.");

        if (string.IsNullOrWhiteSpace(userId))
            return Fail("User id is required.");

        if (request is null)
            return Fail("Request is required.");

        var validationError = ValidateUpdateUserRequest(request);
        if (!string.IsNullOrWhiteSpace(validationError))
            return Fail(validationError);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId && x.TenantId == tenantId, cancellationToken);

        if (user is null)
            return Fail("User not found.");

        var roleName = request.Role.ToString().Trim();

        if (!AllowedTenantRoles.Contains(roleName))
            return Fail("Selected role is not allowed.");

        var isSchoolAdmin = IsSchoolAdmin(roleName);

        var branchValidation = await BuildValidBranchAccessAsync(
            tenantId,
            request.DefaultBranchId,
            request.AllowedBranchIds,
            request.HasAllBranchesAccess || isSchoolAdmin,
            cancellationToken);

        if (!branchValidation.Success)
            return Fail(branchValidation.Message);

        var transactionStarted = false;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            transactionStarted = true;

            user.FullName = request.FullName.Trim();
            user.PhoneNumber = string.IsNullOrWhiteSpace(request.MobileNumber)
                ? null
                : request.MobileNumber.Trim();
            user.BranchId = request.DefaultBranchId;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await SafeRollbackAsync(transactionStarted, cancellationToken);
                return Fail("User update failed.", updateResult.Errors.Select(x => x.Description));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeRoleResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRoleResult.Succeeded)
                {
                    await SafeRollbackAsync(transactionStarted, cancellationToken);
                    return Fail("Role cleanup failed.", removeRoleResult.Errors.Select(x => x.Description));
                }
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                await SafeRollbackAsync(transactionStarted, cancellationToken);
                return Fail("Role update failed.", roleResult.Errors.Select(x => x.Description));
            }

            await ReplaceBranchAccessesAsync(
                tenantId,
                user.Id,
                request.DefaultBranchId,
                branchValidation.BranchIds,
                request.HasAllBranchesAccess || isSchoolAdmin,
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return (true, new
            {
                message = "User updated successfully.",
                userId = user.Id,
                role = roleName,
                defaultBranchId = user.BranchId
            });
        }
        catch (Exception ex)
        {
            if (transactionStarted)
                await SafeRollbackAsync(true, cancellationToken);

            return Fail("An unexpected error occurred while updating the user.", new[] { ex.Message });
        }
    }

    public async Task<(bool Success, object Data)> UpdateUserStatusAsync(
        int tenantId,
        string userId,
        UpdateTenantUserStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0)
            return Fail("Invalid tenant context.");

        if (string.IsNullOrWhiteSpace(userId))
            return Fail("User id is required.");

        if (request is null)
            return Fail("Request is required.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId && x.TenantId == tenantId, cancellationToken);

        if (user is null)
            return Fail("User not found.");

        user.IsActive = request.IsActive;

        var transactionStarted = false;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            transactionStarted = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await SafeRollbackAsync(transactionStarted, cancellationToken);
                return Fail("User status update failed.", updateResult.Errors.Select(x => x.Description));
            }

            var accesses = await _userBranchAccessRepository.GetWhereAsync(
                x => x.TenantId == tenantId && x.UserId == user.Id,
                cancellationToken);

            foreach (var access in accesses)
            {
                access.IsActive = request.IsActive;
                _userBranchAccessRepository.Update(access);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return (true, new
            {
                message = request.IsActive
                    ? "User activated successfully."
                    : "User deactivated successfully.",
                userId = user.Id,
                isActive = user.IsActive
            });
        }
        catch (Exception ex)
        {
            if (transactionStarted)
                await SafeRollbackAsync(true, cancellationToken);

            return Fail("An unexpected error occurred while updating user status.", new[] { ex.Message });
        }
    }

    public async Task<(bool Success, object Data)> DeleteUserAsync(
        int tenantId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0)
            return Fail("Invalid tenant context.");

        if (string.IsNullOrWhiteSpace(userId))
            return Fail("User id is required.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId && x.TenantId == tenantId, cancellationToken);

        if (user is null)
            return Fail("User not found.");

        var transactionStarted = false;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            transactionStarted = true;

            user.IsActive = false;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await SafeRollbackAsync(transactionStarted, cancellationToken);
                return Fail("User delete failed.", updateResult.Errors.Select(x => x.Description));
            }

            var accesses = await _userBranchAccessRepository.GetWhereAsync(
                x => x.TenantId == tenantId && x.UserId == user.Id,
                cancellationToken);

            foreach (var access in accesses)
            {
                access.IsActive = false;
                _userBranchAccessRepository.Update(access);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return (true, new
            {
                message = "User deleted successfully.",
                userId = user.Id
            });
        }
        catch (Exception ex)
        {
            if (transactionStarted)
                await SafeRollbackAsync(true, cancellationToken);

            return Fail("An unexpected error occurred while deleting the user.", new[] { ex.Message });
        }
    }

    private async Task<(bool Success, string Message, List<int> BranchIds)> BuildValidBranchAccessAsync(
        int tenantId,
        int defaultBranchId,
        List<int>? requestedBranchIds,
        bool hasAllBranchesAccess,
        CancellationToken cancellationToken)
    {
        if (defaultBranchId <= 0)
            return (false, "Default branch is required.", new List<int>());

        var allTenantBranches = await _branchRepository.GetWhereAsync(
            x => x.TenantId == tenantId && x.IsActive,
            cancellationToken);

        if (!allTenantBranches.Any())
            return (false, "No active branches found for this tenant.", new List<int>());

        if (!allTenantBranches.Any(x => x.Id == defaultBranchId))
            return (false, "Default branch is invalid or inactive.", new List<int>());

        var finalBranchIds = hasAllBranchesAccess
            ? new List<int> { defaultBranchId }
            : (requestedBranchIds ?? new List<int>())
                .Append(defaultBranchId)
                .Where(x => x > 0)
                .Distinct()
                .ToList();

        if (!finalBranchIds.Any())
            return (false, "At least one allowed branch is required.", new List<int>());

        var invalidBranchIds = finalBranchIds
            .Except(allTenantBranches.Select(x => x.Id))
            .ToList();

        if (invalidBranchIds.Any())
            return (false, "One or more selected branches are invalid or inactive.", new List<int>());

        if (!finalBranchIds.Contains(defaultBranchId))
            return (false, "Default branch must be included in allowed branches.", new List<int>());

        return (true, string.Empty, finalBranchIds);
    }

    private async Task AddBranchAccessesAsync(
        int tenantId,
        string userId,
        int defaultBranchId,
        List<int> branchIds,
        bool hasAllBranchesAccess,
        CancellationToken cancellationToken)
    {
        var accesses = branchIds
            .Distinct()
            .Select(branchId => new UserBranchAccess
            {
                TenantId = tenantId,
                UserId = userId,
                BranchId = branchId,
                HasAllBranchesAccess = hasAllBranchesAccess,
                IsDefault = branchId == defaultBranchId,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            })
            .ToList();

        await _userBranchAccessRepository.AddRangeAsync(accesses, cancellationToken);
    }

    private async Task ReplaceBranchAccessesAsync(
        int tenantId,
        string userId,
        int defaultBranchId,
        List<int> branchIds,
        bool hasAllBranchesAccess,
        CancellationToken cancellationToken)
    {
        var oldAccesses = await _userBranchAccessRepository.GetWhereAsync(
            x => x.TenantId == tenantId && x.UserId == userId,
            cancellationToken);

        foreach (var access in oldAccesses)
        {
            _userBranchAccessRepository.Delete(access);
        }

        await AddBranchAccessesAsync(
            tenantId,
            userId,
            defaultBranchId,
            branchIds,
            hasAllBranchesAccess,
            cancellationToken);
    }

    private static string? ValidateCreateUserRequest(CreateTenantUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return "Full name is required.";

        if (string.IsNullOrWhiteSpace(request.Email))
            return "Email is required.";

        if (string.IsNullOrWhiteSpace(request.Password))
            return "Password is required.";

        if (request.Password.Length < 6)
            return "Password must be at least 6 characters.";

        if (request.DefaultBranchId <= 0)
            return "Default branch is required.";

        if (!request.HasAllBranchesAccess &&
            (request.AllowedBranchIds is null || request.AllowedBranchIds.Count == 0))
            return "At least one allowed branch is required.";

        return null;
    }

    private static string? ValidateUpdateUserRequest(UpdateTenantUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return "Full name is required.";

        if (request.DefaultBranchId <= 0)
            return "Default branch is required.";

        if (!request.HasAllBranchesAccess &&
            (request.AllowedBranchIds is null || request.AllowedBranchIds.Count == 0))
            return "At least one allowed branch is required.";

        return null;
    }

    private async Task SafeRollbackAsync(
        bool transactionStarted,
        CancellationToken cancellationToken = default)
    {
        if (!transactionStarted)
            return;

        try
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        }
        catch
        {
            // Prevent rollback failure from masking original exception
        }
    }

    private static bool IsSchoolAdmin(string roleName)
    {
        return roleName.Equals("SchoolAdmin", StringComparison.OrdinalIgnoreCase);
    }

    private static (bool Success, object Data) Fail(
        string message,
        IEnumerable<string>? errors = null)
    {
        return (false, new
        {
            message,
            errors = errors?.ToList() ?? new List<string>()
        });
    }
}