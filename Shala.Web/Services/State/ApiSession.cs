using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace Shala.Web.Services.State;

/// <summary>
/// Stores the authenticated tenant session.
/// Final scope model: one login = one fixed branch from JWT/session.
/// No runtime branch switching and no all-branches mode in normal pages.
/// </summary>
public class ApiSession
{
    private const string TokenKey = "auth.token";
    private const string EmailKey = "auth.email";
    private const string RoleKey = "auth.role";
    private const string FullNameKey = "auth.fullName";

    private const string TenantIdKey = "auth.tenantId";
    private const string TenantNameKey = "auth.tenantName";

    private const string BranchIdKey = "auth.branchId";
    private const string BranchNameKey = "auth.branchName";
    private const string BranchCodeKey = "auth.branchCode";

    private readonly ProtectedSessionStorage _storage;

    public ApiSession(ProtectedSessionStorage storage)
    {
        _storage = storage;
    }

    public string? Token { get; private set; }
    public string? Email { get; private set; }
    public string? Role { get; private set; }
    public string? FullName { get; private set; }

    public int? TenantId { get; private set; }
    public string? TenantName { get; private set; }

    public int? BranchId { get; private set; }
    public string? BranchName { get; private set; }
    public string? BranchCode { get; private set; }

    // Backward-compatible aliases for pages that still read selected branch display fields.
    public int? SelectedBranchId => BranchId;
    public string? SelectedBranchName => BranchName;
    public string? SelectedBranchCode => BranchCode;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    public bool IsInitialized { get; private set; }

    public bool IsTenantWideRole =>
        RoleEquals("SchoolAdmin") ||
        RoleEquals("TenantAdmin") ||
        RoleEquals("TenantOwner") ||
        RoleEquals("Owner") ||
        RoleEquals("Admin") ||
        RoleEquals("SuperAdmin");

    public async Task<bool> InitializeAsync()
    {
        try
        {
            var tokenResult = await _storage.GetAsync<string>(TokenKey);
            var emailResult = await _storage.GetAsync<string>(EmailKey);
            var roleResult = await _storage.GetAsync<string>(RoleKey);
            var fullNameResult = await _storage.GetAsync<string>(FullNameKey);

            var tenantIdResult = await _storage.GetAsync<int?>(TenantIdKey);
            var tenantNameResult = await _storage.GetAsync<string>(TenantNameKey);

            var branchIdResult = await _storage.GetAsync<int?>(BranchIdKey);
            var branchNameResult = await _storage.GetAsync<string>(BranchNameKey);
            var branchCodeResult = await _storage.GetAsync<string>(BranchCodeKey);

            Token = tokenResult.Success ? Normalize(tokenResult.Value) : null;
            Email = emailResult.Success ? Normalize(emailResult.Value) : null;
            Role = roleResult.Success ? Normalize(roleResult.Value) : null;
            FullName = fullNameResult.Success ? Normalize(fullNameResult.Value) : null;

            TenantId = tenantIdResult.Success ? tenantIdResult.Value : null;
            TenantName = tenantNameResult.Success ? Normalize(tenantNameResult.Value) : null;

            BranchId = branchIdResult.Success ? branchIdResult.Value : null;
            BranchName = branchNameResult.Success ? Normalize(branchNameResult.Value) : null;
            BranchCode = branchCodeResult.Success ? Normalize(branchCodeResult.Value) : null;

            IsInitialized = true;
            return true;
        }
        catch (InvalidOperationException ex) when (IsPrerenderInteropException(ex))
        {
            ResetState();
            return false;
        }
        catch (JSDisconnectedException)
        {
            ResetState();
            return false;
        }
        catch (TaskCanceledException)
        {
            ResetState();
            return false;
        }
    }

    public async Task SetSessionAsync(
        string token,
        string? email,
        string? role,
        string? fullName,
        int? tenantId,
        string? tenantName,
        int? branchId,
        string? branchName,
        string? branchCode)
    {
        Token = Normalize(token);
        Email = Normalize(email);
        Role = Normalize(role);
        FullName = Normalize(fullName);

        TenantId = tenantId;
        TenantName = Normalize(tenantName);

        BranchId = branchId;
        BranchName = Normalize(branchName);
        BranchCode = Normalize(branchCode);

        IsInitialized = true;

        await PersistSessionAsync();
    }

    public Task SetSessionAsync(
        string token,
        string? email,
        string? role,
        string? fullName,
        int? tenantId,
        string? tenantName)
    {
        return SetSessionAsync(token, email, role, fullName, tenantId, tenantName, null, null, null);
    }

    public Task SetSessionAsync(
        string token,
        string? email,
        string? role,
        string? fullName,
        int? tenantId,
        string? tenantName,
        int? branchId)
    {
        return SetSessionAsync(token, email, role, fullName, tenantId, tenantName, branchId, null, null);
    }

    public async Task ClearAsync()
    {
        ResetState();

        try
        {
            await _storage.DeleteAsync(TokenKey);
            await _storage.DeleteAsync(EmailKey);
            await _storage.DeleteAsync(RoleKey);
            await _storage.DeleteAsync(FullNameKey);

            await _storage.DeleteAsync(TenantIdKey);
            await _storage.DeleteAsync(TenantNameKey);

            await _storage.DeleteAsync(BranchIdKey);
            await _storage.DeleteAsync(BranchNameKey);
            await _storage.DeleteAsync(BranchCodeKey);

            // Cleanup keys from old branch-switch implementation.
            await _storage.DeleteAsync("auth.isAllBranches");
            await _storage.DeleteAsync("auth.availableBranches");
        }
        catch (InvalidOperationException ex) when (IsPrerenderInteropException(ex))
        {
        }
        catch (JSDisconnectedException)
        {
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async Task PersistSessionAsync()
    {
        await _storage.SetAsync(TokenKey, Token ?? string.Empty);
        await _storage.SetAsync(EmailKey, Email ?? string.Empty);
        await _storage.SetAsync(RoleKey, Role ?? string.Empty);
        await _storage.SetAsync(FullNameKey, FullName ?? string.Empty);

        await _storage.SetAsync(TenantIdKey, TenantId);
        await _storage.SetAsync(TenantNameKey, TenantName ?? string.Empty);

        await _storage.SetAsync(BranchIdKey, BranchId);
        await _storage.SetAsync(BranchNameKey, BranchName ?? string.Empty);
        await _storage.SetAsync(BranchCodeKey, BranchCode ?? string.Empty);
    }

    private void ResetState()
    {
        Token = null;
        Email = null;
        Role = null;
        FullName = null;

        TenantId = null;
        TenantName = null;

        BranchId = null;
        BranchName = null;
        BranchCode = null;

        IsInitialized = false;
    }

    private bool RoleEquals(string role)
    {
        return Role?.Equals(role, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static bool IsPrerenderInteropException(InvalidOperationException ex)
    {
        return ex.Message.Contains(
            "JavaScript interop calls cannot be issued at this time",
            StringComparison.OrdinalIgnoreCase);
    }
}
