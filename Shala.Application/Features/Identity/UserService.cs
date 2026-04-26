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
        "TenantAdmin",
        "BranchAdmin",
        "Teacher",
        "Accountant",
        "Clerk",
        "TenantUser"
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

    public async Task<(bool Success, object Data)> CreateUserAsync(int tenantId, CreateTenantUserRequest req)
    {
        if (tenantId <= 0)
            return Fail("Invalid tenant context.");

        if (req is null)
            return Fail("Request is required.");

        var validationError = ValidateCreateUserRequest(req);
        if (!string.IsNullOrWhiteSpace(validationError))
            return Fail(validationError);

        var normalizedEmail = req.Email.Trim();
        var normalizedFullName = req.FullName.Trim();
        var normalizedPhone = string.IsNullOrWhiteSpace(req.MobileNumber)
            ? null
            : req.MobileNumber.Trim();

        var roleName = req.Role.ToString().Trim();

        if (!AllowedTenantRoles.Contains(roleName))
            return Fail("Selected role is not allowed.");

        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser is not null)
            return Fail("Email already exists.");

        var allowedBranchIds = req.AllowedBranchIds
            .Append(req.DefaultBranchId)
            .Where(x => x > 0)
            .Distinct()
            .ToList();

        var validBranchIds = await _branchRepository
            .GetAllAsync()
            .ContinueWith(task => task.Result
                .Where(x => x.TenantId == tenantId && x.IsActive && allowedBranchIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToList());

        if (validBranchIds.Count != allowedBranchIds.Count)
            return Fail("One or more selected branches are invalid or inactive.");

        var user = new ApplicationUser
        {
            FullName = normalizedFullName,
            UserName = normalizedEmail,
            Email = normalizedEmail,
            PhoneNumber = normalizedPhone,
            TenantId = tenantId,
            BranchId = req.DefaultBranchId,
            IsActive = true,
            EmailConfirmed = true
        };

        var transactionStarted = false;

        try
        {
            await _unitOfWork.BeginTransactionAsync();
            transactionStarted = true;

            var createResult = await _userManager.CreateAsync(user, req.Password);
            if (!createResult.Succeeded)
            {
                await SafeRollbackAsync(transactionStarted);
                return Fail("User creation failed.", createResult.Errors.Select(x => x.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                await SafeRollbackAsync(transactionStarted);
                return Fail("Role assignment failed.", roleResult.Errors.Select(x => x.Description));
            }

            var branchAccesses = validBranchIds.Select(branchId => new UserBranchAccess
            {
                UserId = user.Id,
                BranchId = branchId,
                IsDefault = branchId == req.DefaultBranchId,
                IsActive = true
            }).ToList();

            await _userBranchAccessRepository.AddRangeAsync(branchAccesses);
            await _unitOfWork.CommitTransactionAsync();

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
                await SafeRollbackAsync(true);

            var createdUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (createdUser is not null)
            {
                try
                {
                    await _userManager.DeleteAsync(createdUser);
                }
                catch
                {
                    // Intentionally swallowed to avoid masking original exception
                }
            }

            return Fail("An unexpected error occurred while creating the user.", new[] { ex.Message });
        }
    }

    public async Task<(bool Success, object Data)> GetUsersAsync(int tenantId)
    {
        if (tenantId <= 0)
            return Fail("Invalid tenant context.");

        var users = await _userManager.Users
            .Where(x => x.TenantId == tenantId)
            .Include(x => x.Branch)
            .Include(x => x.BranchAccesses)
                .ThenInclude(x => x.Branch)
            .AsNoTracking()
            .ToListAsync();

        var result = new List<UserListItemResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

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
                AllowedBranchIds = user.BranchAccesses
                    .Where(x => x.IsActive)
                    .Select(x => x.BranchId)
                    .Distinct()
                    .ToList(),
                AllowedBranchNames = user.BranchAccesses
                    .Where(x => x.IsActive && x.Branch != null)
                    .Select(x => x.Branch.Name)
                    .Distinct()
                    .ToList()
            });
        }

        return (true, result);
    }

    private static string? ValidateCreateUserRequest(CreateTenantUserRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.FullName))
            return "Full name is required.";

        if (string.IsNullOrWhiteSpace(req.Email))
            return "Email is required.";

        if (string.IsNullOrWhiteSpace(req.Password))
            return "Password is required.";

        if (req.DefaultBranchId <= 0)
            return "Default branch is required.";

        if (req.AllowedBranchIds is null || req.AllowedBranchIds.Count == 0)
            return "At least one allowed branch is required.";

        return null;
    }

    private async Task SafeRollbackAsync(bool transactionStarted)
    {
        if (!transactionStarted)
            return;

        try
        {
            await _unitOfWork.RollbackTransactionAsync();
        }
        catch
        {
            // Prevent rollback failure from masking original exception
        }
    }

    private static (bool Success, object Data) Fail(string message, IEnumerable<string>? errors = null)
    {
        return (false, new
        {
            message,
            errors = errors?.ToList() ?? new List<string>()
        });
    }
}