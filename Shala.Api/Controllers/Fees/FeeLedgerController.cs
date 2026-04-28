using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Fees;
using Shala.Shared.Common;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Api.Controllers.Fees;

[ApiController]
[Route("api/fees/ledger")]
public sealed class FeeLedgerController : TenantApiControllerBase
{
    private readonly IFeeLedgerService _feeLedgerService;

    public FeeLedgerController(
        IFeeLedgerService feeLedgerService,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _feeLedgerService = feeLedgerService;
    }

    [HttpPost("dashboard")]
    public async Task<ActionResult<ApiResponse<FeeLedgerDashboardResponse>>> GetDashboardAsync(
        [FromBody] FeeLedgerDashboardRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(ApiResponse<FeeLedgerDashboardResponse>.Fail("Request is required."));
        }

        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _feeLedgerService.GetDashboardAsync(
            TenantId,
            branchId,
            request,
            cancellationToken);

        return Ok(ApiResponse<FeeLedgerDashboardResponse>.Ok(result));
    }
}