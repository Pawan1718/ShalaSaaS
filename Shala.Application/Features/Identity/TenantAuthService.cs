using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Application.Contracts.Jwt;
using Shala.Domain.Entities.Identity;
using Shala.Domain.Entities.Organization;
using Shala.Domain.Entities.Platform;
using Shala.Shared.Requests.Identity;
using Shala.Shared.Responses.Identity;

namespace Shala.Application.Features.Identity;

public sealed class TenantAuthService : ITenantAuthService
{
    private const string InvalidLoginMessage = "Invalid username or password.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IGenericRepository<ApplicationUser> _userRepository;
    private readonly IGenericRepository<SchoolTenant> _tenantRepository;
    private readonly IGenericRepository<Branch> _branchRepository;
    private readonly IGenericRepository<UserBranchAccess> _userBranchAccessRepository;

    public TenantAuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IGenericRepository<ApplicationUser> userRepository,
        IGenericRepository<SchoolTenant> tenantRepository,
        IGenericRepository<Branch> branchRepository,
        IGenericRepository<UserBranchAccess> userBranchAccessRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _branchRepository = branchRepository;
        _userBranchAccessRepository = userBranchAccessRepository;
    }

    public async Task<(bool Success, PlatformLoginResponse? Data, string Message)> LoginAsync(
        TenantLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            return (false, null, InvalidLoginMessage);

        if (string.IsNullOrWhiteSpace(request.UserIdOrEmail) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return (false, null, InvalidLoginMessage);
        }

        var input = request.UserIdOrEmail.Trim();

        var user = await _userRepository.FirstOrDefaultAsync(
            x => x.Email == input || x.UserName == input,
            cancellationToken);

        if (user is null || !user.IsActive || !user.TenantId.HasValue)
            return (false, null, InvalidLoginMessage);

        var tenant = await _tenantRepository.FirstOrDefaultAsync(
            x => x.Id == user.TenantId.Value && x.IsActive,
            cancellationToken);

        if (tenant is null)
            return (false, null, InvalidLoginMessage);

        if (await _userManager.IsLockedOutAsync(user))
            return (false, null, InvalidLoginMessage);

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!signInResult.Succeeded)
            return (false, null, InvalidLoginMessage);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "TenantUser";

        var accessRows = await _userBranchAccessRepository.GetWhereAsync(
            x => x.UserId == user.Id &&
                 x.TenantId == user.TenantId.Value &&
                 x.IsActive,
            query => query.Include(x => x.Branch),
            query => query
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.BranchId),
            cancellationToken);

        if (accessRows.Count == 0)
            return (false, null, "No active branch is assigned to this user.");

        var hasAllBranchesAccess = accessRows.Any(x => x.HasAllBranchesAccess);

        var allowedBranches = hasAllBranchesAccess
            ? await _branchRepository.GetWhereAsync(
                x => x.TenantId == user.TenantId.Value && x.IsActive,
                query => query,
                query => query
                    .OrderByDescending(x => x.IsMainBranch)
                    .ThenBy(x => x.Name),
                cancellationToken)
            : accessRows
                .Where(x => x.Branch is not null &&
                            x.Branch.IsActive &&
                            x.Branch.TenantId == user.TenantId.Value)
                .OrderByDescending(x => x.Branch!.IsMainBranch)
                .ThenByDescending(x => x.IsDefault)
                .ThenBy(x => x.Branch!.Name)
                .Select(x => x.Branch!)
                .DistinctBy(x => x.Id)
                .ToList();

        if (allowedBranches.Count == 0)
            return (false, null, "No active branch is assigned to this user.");

        if (!string.IsNullOrWhiteSpace(request.BranchCode))
        {
            var selectedBranch = allowedBranches.FirstOrDefault(x =>
                x.Code.Equals(
                    request.BranchCode.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (selectedBranch is null)
                return (false, null, "Invalid branch selection.");

            return await BuildLoginSuccessAsync(
                user,
                tenant.Id,
                tenant.Name,
                role,
                selectedBranch,
                allowedBranches,
                accessRows,
                cancellationToken);
        }

        var defaultBranchId = user.BranchId;
        var defaultBranch =
            (defaultBranchId.HasValue
                ? allowedBranches.FirstOrDefault(x => x.Id == defaultBranchId.Value)
                : null) ??
            allowedBranches.FirstOrDefault(x => x.IsMainBranch) ??
            allowedBranches.First();

        return await BuildLoginSuccessAsync(
            user,
            tenant.Id,
            tenant.Name,
            role,
            defaultBranch,
            allowedBranches,
            accessRows,
            cancellationToken);
    }

    private async Task<(bool Success, PlatformLoginResponse? Data, string Message)> BuildLoginSuccessAsync(
        ApplicationUser user,
        int tenantId,
        string tenantName,
        string role,
        Branch branch,
        IReadOnlyList<Branch> allowedBranches,
        IReadOnlyList<UserBranchAccess> accessRows,
        CancellationToken cancellationToken)
    {
        var token = await _jwtTokenService.GenerateTenantTokenAsync(
            user,
            role,
            tenantId,
            branch.Id,
            cancellationToken);

        return (true, new PlatformLoginResponse
        {
            Token = token,
            Email = user.Email ?? string.Empty,
            Role = role,
            FullName = user.FullName,
            TenantId = tenantId,
            TenantName = tenantName,
            BranchId = branch.Id,
            BranchName = branch.Name,
            BranchCode = branch.Code,

            // Important:
            // Multi-branch user ko bhi direct login karna hai.
            // Branch selection ab sidebar se hoga.
            RequiresBranchSelection = false,

            BranchOptions = allowedBranches.Select(x => new LoginBranchOption
            {
                BranchId = x.Id,
                BranchCode = x.Code,
                BranchName = x.Name,
                IsMainBranch = x.IsMainBranch
            }).ToList()
        }, "Login successful.");
    }
}