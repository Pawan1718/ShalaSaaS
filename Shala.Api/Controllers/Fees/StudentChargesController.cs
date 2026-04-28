using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Api.Controllers.Fees;

[Route("api/fees/charges")]
public class StudentChargesController : TenantApiControllerBase
{
    private readonly IStudentChargeService _service;

    public StudentChargesController(
        IStudentChargeService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<IActionResult> GetByStudentId(
        int studentId,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var data = await _service.GetByStudentIdAsync(
            TenantId,
            branchId,
            studentId,
            cancellationToken);

        var result = data.Select(MapCharge).ToList();

        return Ok(result);
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _service.MarkCancelledAsync(
            TenantId,
            branchId,
            id,
            cancellationToken);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    private static StudentChargeResponse MapCharge(Domain.Entities.Fees.StudentCharge x)
    {
        return new StudentChargeResponse
        {
            Id = x.Id,
            StudentId = x.StudentId ?? 0,
            StudentAdmissionId = x.StudentAdmissionId ?? 0,
            StudentFeeAssignmentId = x.StudentFeeAssignmentId ?? 0,
            FeeHeadId = x.FeeHeadId,
            ChargeLabel = x.ChargeLabel,
            PeriodLabel = x.PeriodLabel,
            DueDate = x.DueDate,
            Amount = x.Amount,
            DiscountAmount = x.DiscountAmount,
            FineAmount = x.FineAmount,
            PaidAmount = x.PaidAmount,
            NetAmount = x.NetAmount,
            BalanceAmount = x.BalanceAmount,
            IsSettled = x.IsSettled,
            IsCancelled = x.IsCancelled
        };
    }
}