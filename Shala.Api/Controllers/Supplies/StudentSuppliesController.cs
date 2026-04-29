using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Supplies;
using Shala.Shared.Requests.Supplies;

namespace Shala.Api.Controllers.Supplies;

[Route("api/tenant/supplies")]
public class StudentSuppliesController : TenantApiControllerBase
{
    private readonly IStudentSupplyService _service;

    public StudentSuppliesController(
        IStudentSupplyService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(
        [FromQuery] int? academicYearId,
        CancellationToken cancellationToken)
    {
        var data = await _service.GetDashboardAsync(
            TenantId,
            BranchId,
            academicYearId,
            cancellationToken);

        return Ok(data);
    }

    [HttpPost("stock-in")]
    public async Task<IActionResult> AddStock(
        [FromBody] AddSupplyStockRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.AddStockAsync(
            TenantId,
            BranchId,
            Actor,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("issues")]
    public async Task<IActionResult> CreateIssue(
        [FromBody] CreateStudentSupplyIssueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateIssueAsync(
            TenantId,
            BranchId,
            Actor,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("issues")]
    public async Task<IActionResult> GetRecentIssues(
        [FromQuery] int? academicYearId,
        CancellationToken cancellationToken)
    {
        var data = await _service.GetRecentIssuesAsync(
            TenantId,
            BranchId,
            academicYearId,
            cancellationToken);

        return Ok(data);
    }

    [HttpGet("issues/{id:int}")]
    public async Task<IActionResult> GetIssueDetail(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await _service.GetIssueDetailAsync(
            TenantId,
            BranchId,
            id,
            cancellationToken);

        if (data is null)
            return NotFound("Supply bill not found.");

        return Ok(data);
    }

    [HttpPost("issues/{id:int}/payments")]
    public async Task<IActionResult> CollectDue(
        int id,
        [FromBody] CollectSupplyDueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CollectDueAsync(
            TenantId,
            BranchId,
            Actor,
            id,
            request,
            cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("students/{studentId:int}/history")]
    public async Task<IActionResult> StudentHistory(
        int studentId,
        [FromQuery] int? academicYearId,
        CancellationToken cancellationToken)
    {
        var data = await _service.GetStudentHistoryAsync(
            TenantId,
            BranchId,
            studentId,
            academicYearId,
            cancellationToken);

        return Ok(data);
    }

    [HttpGet("dues")]
    public async Task<IActionResult> PendingDues(
        [FromQuery] int? academicYearId,
        [FromQuery] int? studentId,
        CancellationToken cancellationToken)
    {
        var data = await _service.GetPendingDuesAsync(
            TenantId,
            BranchId,
            academicYearId,
            studentId,
            cancellationToken);

        return Ok(data);
    }

    [HttpGet("reports/low-stock")]
    public async Task<IActionResult> LowStock(
        CancellationToken cancellationToken)
    {
        var data = await _service.GetLowStockAsync(
            TenantId,
            BranchId,
            cancellationToken);

        return Ok(data);
    }

    [HttpGet("reports/stock-history")]
    public async Task<IActionResult> StockHistory(
        [FromQuery] SupplyReportRequest request,
        CancellationToken cancellationToken)
    {
        var data = await _service.GetStockHistoryAsync(
            TenantId,
            BranchId,
            request,
            cancellationToken);

        return Ok(data);
    }
}