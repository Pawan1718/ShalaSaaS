using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace Shala.Web.Services.State;

public class ApiSession
{
    private const string TokenKey = "auth.token";
    private const string EmailKey = "auth.email";
    private const string RoleKey = "auth.role";
    private const string TenantIdKey = "auth.tenantId";
    private const string BranchIdKey = "auth.branchId";
    private const string FullNameKey = "auth.fullName";

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
    public int? BranchId { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    public bool IsInitialized { get; private set; }

    public async Task<bool> InitializeAsync()
    {
        try
        {
            var tokenResult = await _storage.GetAsync<string>(TokenKey);
            var emailResult = await _storage.GetAsync<string>(EmailKey);
            var roleResult = await _storage.GetAsync<string>(RoleKey);
            var fullNameResult = await _storage.GetAsync<string>(FullNameKey);
            var tenantIdResult = await _storage.GetAsync<int?>(TenantIdKey);
            var branchIdResult = await _storage.GetAsync<int?>(BranchIdKey);

            Token = tokenResult.Success ? Normalize(tokenResult.Value) : null;
            Email = emailResult.Success ? Normalize(emailResult.Value) : null;
            Role = roleResult.Success ? Normalize(roleResult.Value) : null;
            FullName = fullNameResult.Success ? Normalize(fullNameResult.Value) : null;
            TenantId = tenantIdResult.Success ? tenantIdResult.Value : null;
            BranchId = branchIdResult.Success ? branchIdResult.Value : null;

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
        int? branchId)
    {
        Token = Normalize(token);
        Email = Normalize(email);
        Role = Normalize(role);
        FullName = Normalize(fullName);
        TenantId = tenantId;
        BranchId = branchId;
        IsInitialized = true;

        await _storage.SetAsync(TokenKey, token);
        await _storage.SetAsync(EmailKey, email ?? string.Empty);
        await _storage.SetAsync(RoleKey, role ?? string.Empty);
        await _storage.SetAsync(FullNameKey, fullName ?? string.Empty);
        await _storage.SetAsync(TenantIdKey, tenantId);
        await _storage.SetAsync(BranchIdKey, branchId);
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
            await _storage.DeleteAsync(BranchIdKey);
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

    private void ResetState()
    {
        Token = null;
        Email = null;
        Role = null;
        FullName = null;
        TenantId = null;
        BranchId = null;
        IsInitialized = false;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static bool IsPrerenderInteropException(InvalidOperationException ex)
    {
        return ex.Message.Contains("JavaScript interop calls cannot be issued at this time", StringComparison.OrdinalIgnoreCase);
    }
}