# Single-Branch Operations + Admin All-Branch Dashboard Changes

## Model applied
- Removed runtime branch switching from the tenant layout.
- Tenant session now stores only fixed JWT/session branch data: TenantId, TenantName, BranchId, BranchName, BranchCode.
- Tenant login no longer asks for branch selection or loads available branches into session.
- Tenant dashboard now requests all-branch dashboard automatically for tenant-wide roles and own-branch dashboard for normal branch users.
- Backend tenant dashboard now treats `SchoolAdmin`/tenant-wide roles as all-branch dashboard roles.
- Tenant user creation API now enforces:
  - SchoolAdmin/tenant-wide roles can create users for tenant branches.
  - BranchAdmin can create users only for their own branch.
  - Teacher/Staff/Accountant cannot create users.

## Important files changed
- Shala.Web/Services/State/ApiSession.cs
- Shala.Web/Components/Layouts/TenantMainLayout.razor
- Shala.Web/Pages/Tenant/TenantLogin.razor
- Shala.Web/Pages/Tenant/TenantDashboard.razor
- Shala.Api/Controllers/Tenant/UsersController.cs
- Shala.Application/Features/Identity/IUserService.cs
- Shala.Application/Features/Identity/UserService.cs
- Shala.Api/Controllers/Tenant/TenantBranchesController.cs
- Shala.Infrastructure/Repositories/Tenant/TenantDashboardRepository.cs

## Notes
- `TenantBranchRepository` is kept registered for other branch management screens, but it is no longer used by login/session switching.
- `dotnet build` could not be run in this environment because the .NET SDK is not installed here. Please run `dotnet build` locally after extracting.
