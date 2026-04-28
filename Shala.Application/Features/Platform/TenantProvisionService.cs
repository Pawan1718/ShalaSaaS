using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Application.Repositories.Platform;
using Shala.Domain.Entities;
using Shala.Domain.Entities.Organization;
using Shala.Domain.Entities.Platform;
using Shala.Domain.Entities.Identity;
using Shala.Shared.Requests.Platform;
using Shala.Shared.Responses.Platform;

namespace Shala.Application.Features.Platform;

public class TenantProvisionService : ITenantProvisionService
{
    private readonly ITenantProvisionRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBranchCodeGenerator _branchCodeGenerator;

    public TenantProvisionService(
        ITenantProvisionRepository repository,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IBranchCodeGenerator branchCodeGenerator)
    {
        _repository = repository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _branchCodeGenerator = branchCodeGenerator;
    }

    public async Task<(bool Success, object Data)> CreateTenantAsync(CreateTenantRequest req)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (string.IsNullOrWhiteSpace(req.SchoolName))
                return (false, new { message = "School name is required" });

            if (string.IsNullOrWhiteSpace(req.AdminFullName))
                return (false, new { message = "Admin full name is required" });

            if (string.IsNullOrWhiteSpace(req.AdminEmail))
                return (false, new { message = "Admin email is required" });

            if (string.IsNullOrWhiteSpace(req.AdminPassword))
                return (false, new { message = "Admin password is required" });

            var normalizedAdminEmail = req.AdminEmail.Trim();

            var existingAdmin = await _userManager.FindByEmailAsync(normalizedAdminEmail);
            if (existingAdmin is not null)
                return (false, new { message = "Admin email already exists" });

            var tenant = new SchoolTenant
            {
                Name = req.SchoolName.Trim(),
                BusinessCategory = req.BusinessCategory?.Trim() ?? string.Empty,
                Email = req.Email?.Trim() ?? string.Empty,
                MobileNumber = req.MobileNumber?.Trim() ?? string.Empty,
                SubscriptionPlan = string.IsNullOrWhiteSpace(req.SubscriptionPlan)
                    ? "Basic"
                    : req.SubscriptionPlan.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddTenantAsync(tenant);
            await _unitOfWork.SaveChangesAsync();

            var mainBranchCode = await _branchCodeGenerator.GenerateAsync(
                tenant.Id,
                tenant.Name,
                CancellationToken.None);

            var mainBranch = new Branch
            {
                TenantId = tenant.Id,
                Name = tenant.Name,
                Code = mainBranchCode,
                Email = tenant.Email,
                Phone = tenant.MobileNumber,
                PrincipalName = req.AdminFullName.Trim(),
                IsMainBranch = true,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repository.AddBranchAsync(mainBranch);
            await _unitOfWork.SaveChangesAsync();

            var adminUser = new ApplicationUser
            {
                FullName = req.AdminFullName.Trim(),
                UserName = normalizedAdminEmail,
                Email = normalizedAdminEmail,
                PhoneNumber = req.MobileNumber?.Trim(),
                TenantId = tenant.Id,
                BranchId = mainBranch.Id,
                IsActive = true
            };

            var userResult = await _userManager.CreateAsync(adminUser, req.AdminPassword);
            if (!userResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return (false, new
                {
                    message = "Admin user creation failed",
                    errors = userResult.Errors.Select(x => new
                    {
                        code = x.Code,
                        description = x.Description
                    }).ToList()
                });
            }

            var roleResult = await _userManager.AddToRoleAsync(adminUser, "SchoolAdmin");
            if (!roleResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return (false, new
                {
                    message = "Role assignment failed",
                    errors = roleResult.Errors.Select(x => x.Description)
                });
            }

            await _repository.AddUserBranchAccessAsync(new UserBranchAccess
            {
                TenantId = tenant.Id,
                UserId = adminUser.Id,
                BranchId = mainBranch.Id,
                HasAllBranchesAccess = true,
                IsDefault = true,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            });

            await _unitOfWork.CommitTransactionAsync();

            return (true, new CreateTenantResponse
            {
                Message = "Tenant created successfully",
                TenantId = tenant.Id,
                MainBranchId = mainBranch.Id,
                AdminEmail = adminUser.Email ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            return (false, new
            {
                message = "Tenant creation failed",
                error = ex.Message
            });
        }
    }

    public async Task<(bool Success, object Data)> GetTenantsAsync()
    {
        var tenants = await _repository.GetTenantsAsync();
        return (true, tenants);
    }

    public async Task<(bool Success, object Data)> GetTenantsPagedAsync(TenantListRequest req)
    {
        var result = await _repository.GetTenantsPagedAsync(req);
        return (true, result);
    }

    public async Task<(bool Success, object Data)> GetTenantByIdAsync(int tenantId)
    {
        var tenant = await _repository.GetTenantByIdAsync(tenantId);

        if (tenant is null)
            return (false, new { message = "Tenant not found" });

        return (true, tenant);
    }

    public async Task<(bool Success, object Data)> UpdateTenantBasicInfoAsync(
        int tenantId,
        UpdateTenantBasicInfoRequest req)
    {
        if (req is null)
            return (false, new { message = "Invalid request" });

        if (string.IsNullOrWhiteSpace(req.SchoolName))
            return (false, new { message = "School name is required" });

        if (string.IsNullOrWhiteSpace(req.SubscriptionPlan))
            return (false, new { message = "Subscription plan is required" });

        var tenant = await _repository.GetTenantEntityByIdAsync(tenantId);
        if (tenant is null)
            return (false, new { message = "Tenant not found" });

        tenant.Name = req.SchoolName.Trim();
        tenant.SubscriptionPlan = req.SubscriptionPlan.Trim();

        await _unitOfWork.SaveChangesAsync();

        return (true, new
        {
            message = "Tenant basic info updated successfully",
            tenantId = tenant.Id,
            schoolName = tenant.Name,
            subscriptionPlan = tenant.SubscriptionPlan
        });
    }

    public async Task<(bool Success, object Data)> ResetTenantAdminPasswordAsync(
        int tenantId,
        ResetTenantAdminPasswordRequest req)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.NewPassword))
            return (false, new { message = "New password is required" });

        var adminUser = await _userManager.Users
            .Where(x => x.TenantId == tenantId)
            .FirstOrDefaultAsync();

        if (adminUser is null)
            return (false, new { message = "Tenant admin not found" });

        var roles = await _userManager.GetRolesAsync(adminUser);
        if (!roles.Contains("SchoolAdmin"))
            return (false, new { message = "School admin not found" });

        var token = await _userManager.GeneratePasswordResetTokenAsync(adminUser);
        var result = await _userManager.ResetPasswordAsync(adminUser, token, req.NewPassword);

        if (!result.Succeeded)
        {
            return (false, new
            {
                message = "Password reset failed",
                errors = result.Errors.Select(x => x.Description)
            });
        }

        return (true, new
        {
            message = "Tenant admin password reset successfully"
        });
    }

    public async Task<(bool Success, object Data)> UpdateTenantAdminEmailAsync(
        int tenantId,
        UpdateTenantAdminEmailRequest req)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.NewEmail))
            return (false, new { message = "New email is required" });

        var normalizedEmail = req.NewEmail.Trim();

        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser is not null)
            return (false, new { message = "Email already exists" });

        var adminUser = await _userManager.Users
            .Where(x => x.TenantId == tenantId)
            .FirstOrDefaultAsync();

        if (adminUser is null)
            return (false, new { message = "Tenant admin not found" });

        var roles = await _userManager.GetRolesAsync(adminUser);
        if (!roles.Contains("SchoolAdmin"))
            return (false, new { message = "School admin not found" });

        adminUser.Email = normalizedEmail;
        adminUser.UserName = normalizedEmail;

        var result = await _userManager.UpdateAsync(adminUser);

        if (!result.Succeeded)
        {
            return (false, new
            {
                message = "Email update failed",
                errors = result.Errors.Select(x => x.Description)
            });
        }

        return (true, new
        {
            message = "Tenant admin email updated successfully",
            newEmail = normalizedEmail
        });
    }

    public async Task<(bool Success, object Data)> UpdateTenantStatusAsync(
        int tenantId,
        UpdateTenantStatusRequest req)
    {
        var tenant = await _repository.GetTenantEntityByIdAsync(tenantId);
        if (tenant is null)
            return (false, new { message = "Tenant not found" });

        tenant.IsActive = req.IsActive;
        await _unitOfWork.SaveChangesAsync();

        var users = await _userManager.Users
            .Where(x => x.TenantId == tenantId)
            .ToListAsync();

        foreach (var user in users)
        {
            user.IsActive = req.IsActive;
            await _userManager.UpdateAsync(user);
        }

        return (true, new
        {
            message = req.IsActive
                ? "Tenant activated successfully"
                : "Tenant deactivated successfully"
        });
    }
}