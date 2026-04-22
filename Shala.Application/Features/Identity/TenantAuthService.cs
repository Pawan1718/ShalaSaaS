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
    private readonly IGenericRepository<UserBranchAccess> _userBranchAccessRepository;

    public TenantAuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IGenericRepository<ApplicationUser> userRepository,
        IGenericRepository<SchoolTenant> tenantRepository,
        IGenericRepository<UserBranchAccess> userBranchAccessRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
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

        var allowedBranches = await _userBranchAccessRepository.GetWhereAsync(
            x => x.UserId == user.Id &&
                 x.IsActive &&
                 x.Branch.IsActive &&
                 x.Branch.TenantId == user.TenantId.Value,
            query => query.Include(x => x.Branch),
            query => query
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.Branch.IsMainBranch)
                .ThenBy(x => x.Branch.Name),
            cancellationToken);

        if (allowedBranches.Count == 0)
            return (false, null, InvalidLoginMessage);

        if (!string.IsNullOrWhiteSpace(request.BranchCode))
        {
            var selectedAccess = allowedBranches.FirstOrDefault(x =>
                x.Branch.Code.Equals(
                    request.BranchCode.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (selectedAccess is null)
                return (false, null, "Invalid branch selection.");

            return await BuildLoginSuccessAsync(
                user,
                tenant.Id,
                tenant.Name,
                role,
                selectedAccess.Branch,
                cancellationToken);
        }

        if (allowedBranches.Count == 1)
        {
            return await BuildLoginSuccessAsync(
                user,
                tenant.Id,
                tenant.Name,
                role,
                allowedBranches[0].Branch,
                cancellationToken);
        }

        var defaultAccess = allowedBranches.FirstOrDefault(x => x.IsDefault);
        if (defaultAccess is not null)
        {
            return await BuildLoginSuccessAsync(
                user,
                tenant.Id,
                tenant.Name,
                role,
                defaultAccess.Branch,
                cancellationToken);
        }

        return (true, new PlatformLoginResponse
        {
            Email = user.Email ?? string.Empty,
            Role = role,
            FullName = user.FullName,
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            RequiresBranchSelection = true,
            BranchOptions = allowedBranches.Select(x => new LoginBranchOption
            {
                BranchId = x.Branch.Id,
                BranchCode = x.Branch.Code,
                BranchName = x.Branch.Name,
                IsMainBranch = x.Branch.IsMainBranch
            }).ToList()
        }, "Please select a branch.");
    }

    private async Task<(bool Success, PlatformLoginResponse? Data, string Message)> BuildLoginSuccessAsync(
        ApplicationUser user,
        int tenantId,
        string tenantName,
        string role,
        Branch branch,
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
            RequiresBranchSelection = false,
            BranchOptions = new List<LoginBranchOption>()
        }, "Login successful.");
    }
}