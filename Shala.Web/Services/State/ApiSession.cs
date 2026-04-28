using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Shala.Shared.Responses.Tenant;

namespace Shala.Web.Services.State;

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
    private const string IsAllBranchesKey = "auth.isAllBranches";
    private const string AvailableBranchesKey = "auth.availableBranches";

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

    // Backward-compatible property.
    // Existing code that uses Session.BranchId will now respect "All Branches".
    public int? BranchId => CurrentBranchId;

    public int? SelectedBranchId { get; private set; }
    public string? SelectedBranchName { get; private set; }
    public string? SelectedBranchCode { get; private set; }
    public bool IsAllBranchesSelected { get; private set; }

    public int? CurrentBranchId => IsAllBranchesSelected ? null : SelectedBranchId;

    public List<TenantBranchOptionDto> AvailableBranches { get; private set; } = new();

    public bool HasBranches => AvailableBranches.Count > 0;
    public bool HasMultipleBranches => AvailableBranches.Count > 1;
    public bool CanSwitchBranches => HasMultipleBranches;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    public bool IsInitialized { get; private set; }

    public event Action? OnBranchChanged;

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
            var isAllBranchesResult = await _storage.GetAsync<bool>(IsAllBranchesKey);
            var branchesResult = await _storage.GetAsync<List<TenantBranchOptionDto>>(AvailableBranchesKey);

            Token = tokenResult.Success ? Normalize(tokenResult.Value) : null;
            Email = emailResult.Success ? Normalize(emailResult.Value) : null;
            Role = roleResult.Success ? Normalize(roleResult.Value) : null;
            FullName = fullNameResult.Success ? Normalize(fullNameResult.Value) : null;

            TenantId = tenantIdResult.Success ? tenantIdResult.Value : null;
            TenantName = tenantNameResult.Success ? Normalize(tenantNameResult.Value) : null;

            SelectedBranchId = branchIdResult.Success ? branchIdResult.Value : null;
            SelectedBranchName = branchNameResult.Success ? Normalize(branchNameResult.Value) : null;
            SelectedBranchCode = branchCodeResult.Success ? Normalize(branchCodeResult.Value) : null;
            IsAllBranchesSelected = isAllBranchesResult.Success && isAllBranchesResult.Value;

            AvailableBranches = branchesResult.Success && branchesResult.Value is not null
                ? branchesResult.Value
                : new List<TenantBranchOptionDto>();

            NormalizeBranchStateAfterLoad();

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

        SelectedBranchId = branchId;
        SelectedBranchName = Normalize(branchName);
        SelectedBranchCode = Normalize(branchCode);
        IsAllBranchesSelected = false;

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
        return SetSessionAsync(
            token,
            email,
            role,
            fullName,
            tenantId,
            tenantName,
            branchId: null,
            branchName: null,
            branchCode: null);
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
        return SetSessionAsync(
            token,
            email,
            role,
            fullName,
            tenantId,
            tenantName,
            branchId,
            branchName: null,
            branchCode: null);
    }

    public async Task SetAvailableBranchesAsync(List<TenantBranchOptionDto>? branches, bool autoSelect = true)
    {
        AvailableBranches = branches ?? new List<TenantBranchOptionDto>();

        await _storage.SetAsync(AvailableBranchesKey, AvailableBranches);

        if (!autoSelect)
            return;

        await EnsureValidBranchSelectionAsync(notify: false);
    }

    public async Task EnsureValidBranchSelectionAsync(bool notify = false)
    {
        if (AvailableBranches.Count == 0)
        {
            SelectedBranchId = null;
            SelectedBranchName = null;
            SelectedBranchCode = null;
            IsAllBranchesSelected = false;

            await PersistBranchStateAsync();

            if (notify)
                OnBranchChanged?.Invoke();

            return;
        }

        if (IsAllBranchesSelected)
        {
            if (AvailableBranches.Count > 1)
            {
                SelectedBranchId = null;
                SelectedBranchName = "All Branches";
                SelectedBranchCode = null;

                await PersistBranchStateAsync();

                if (notify)
                    OnBranchChanged?.Invoke();

                return;
            }

            IsAllBranchesSelected = false;
        }

        if (SelectedBranchId.HasValue)
        {
            var existing = AvailableBranches.FirstOrDefault(x => x.BranchId == SelectedBranchId.Value);

            if (existing is not null)
            {
                SelectedBranchName = Normalize(existing.BranchName);
                SelectedBranchCode = Normalize(existing.BranchCode);
                IsAllBranchesSelected = false;

                await PersistBranchStateAsync();

                if (notify)
                    OnBranchChanged?.Invoke();

                return;
            }
        }

        await SelectDefaultBranchAsync(notify);
    }

    public async Task SelectDefaultBranchAsync(bool notify = false)
    {
        if (AvailableBranches.Count == 0)
            return;

        var selected =
            AvailableBranches.FirstOrDefault(x => x.IsDefault) ??
            AvailableBranches.FirstOrDefault(x => x.IsMainBranch) ??
            AvailableBranches.First();

        await SelectBranchAsync(selected, notify);
    }

    public async Task SelectBranchAsync(TenantBranchOptionDto branch, bool notify = true)
    {
        SelectedBranchId = branch.BranchId;
        SelectedBranchName = Normalize(branch.BranchName);
        SelectedBranchCode = Normalize(branch.BranchCode);
        IsAllBranchesSelected = false;

        await PersistBranchStateAsync();

        if (notify)
            OnBranchChanged?.Invoke();
    }

    public async Task SelectBranchAsync(int branchId, bool notify = true)
    {
        var branch = AvailableBranches.FirstOrDefault(x => x.BranchId == branchId);

        if (branch is null)
            return;

        await SelectBranchAsync(branch, notify);
    }

    public async Task SelectAllBranchesAsync(bool notify = true)
    {
        if (AvailableBranches.Count <= 1)
            return;

        SelectedBranchId = null;
        SelectedBranchName = "All Branches";
        SelectedBranchCode = null;
        IsAllBranchesSelected = true;

        await PersistBranchStateAsync();

        if (notify)
            OnBranchChanged?.Invoke();
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
            await _storage.DeleteAsync(IsAllBranchesKey);
            await _storage.DeleteAsync(AvailableBranchesKey);
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

        await PersistBranchStateAsync();
    }

    private async Task PersistBranchStateAsync()
    {
        await _storage.SetAsync(BranchIdKey, SelectedBranchId);
        await _storage.SetAsync(BranchNameKey, SelectedBranchName ?? string.Empty);
        await _storage.SetAsync(BranchCodeKey, SelectedBranchCode ?? string.Empty);
        await _storage.SetAsync(IsAllBranchesKey, IsAllBranchesSelected);
    }

    private void NormalizeBranchStateAfterLoad()
    {
        if (AvailableBranches.Count == 0)
        {
            SelectedBranchId = null;
            SelectedBranchName = null;
            SelectedBranchCode = null;
            IsAllBranchesSelected = false;
            return;
        }

        if (IsAllBranchesSelected)
        {
            if (AvailableBranches.Count > 1)
            {
                SelectedBranchId = null;
                SelectedBranchName = "All Branches";
                SelectedBranchCode = null;
                return;
            }

            IsAllBranchesSelected = false;
        }

        if (!SelectedBranchId.HasValue)
            return;

        var branch = AvailableBranches.FirstOrDefault(x => x.BranchId == SelectedBranchId.Value);

        if (branch is null)
        {
            SelectedBranchId = null;
            SelectedBranchName = null;
            SelectedBranchCode = null;
            IsAllBranchesSelected = false;
            return;
        }

        SelectedBranchName = Normalize(branch.BranchName);
        SelectedBranchCode = Normalize(branch.BranchCode);
    }

    private void ResetState()
    {
        Token = null;
        Email = null;
        Role = null;
        FullName = null;

        TenantId = null;
        TenantName = null;

        SelectedBranchId = null;
        SelectedBranchName = null;
        SelectedBranchCode = null;
        IsAllBranchesSelected = false;

        AvailableBranches = new List<TenantBranchOptionDto>();

        IsInitialized = false;
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