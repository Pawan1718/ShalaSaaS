using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Api.Controllers.Fees;

[Route("api/fees/receipts")]
public class FeeReceiptsController : TenantApiControllerBase
{
    private readonly IFeeReceiptService _service;

    public FeeReceiptsController(
        IFeeReceiptService service,
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
        var result = await _service.GetByStudentIdAsync(TenantId, BranchId, studentId, cancellationToken);
        return Ok(result.Select(MapReceipt).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(TenantId, BranchId, id, cancellationToken);

        if (result is null)
            return NotFound(new { message = "Fee receipt not found." });

        return Ok(MapReceipt(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFeeReceiptRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new FeeReceipt
        {
            StudentId = request.StudentId,
            StudentAdmissionId = request.StudentAdmissionId <= 0 ? null : request.StudentAdmissionId,
            ReceiptDate = DateTime.UtcNow,
            PaymentMode = request.PaymentMode,
            TransactionReference = request.TransactionReference,
            Remarks = request.Remarks,
            Allocations = request.Allocations.Select(x => new FeeReceiptAllocation
            {
                StudentChargeId = x.StudentChargeId,
                AllocatedAmount = x.AllocatedAmount
            }).ToList()
        };

        var result = await _service.CreateAsync(TenantId, BranchId, entity, cancellationToken);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(MapReceipt(result.Data!));
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        [FromBody] CancelFeeReceiptRequest? request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CancelReceiptAsync(
            TenantId,
            BranchId,
            id,
            request?.Reason,
            cancellationToken);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    private static FeeReceiptResponse MapReceipt(FeeReceipt x)
    {
        return new FeeReceiptResponse
        {
            Id = x.Id,
            ReceiptNo = x.ReceiptNo,
            StudentId = x.StudentId,
            StudentAdmissionId = x.StudentAdmissionId ?? 0,
            ReceiptDate = x.ReceiptDate,
            PaymentMode = x.PaymentMode,
            TransactionReference = x.TransactionReference,
            Remarks = x.Remarks,
            TotalAmount = x.TotalAmount,
            IsCancelled = x.IsCancelled,
            CancelledOnUtc = x.CancelledOnUtc,
            CancelReason = x.CancelReason,
            Allocations = x.Allocations?
                .Select(a => new FeeReceiptAllocationResponse
                {
                    StudentChargeId = a.StudentChargeId,
                    AllocatedAmount = a.AllocatedAmount
                })
                .ToList() ?? new List<FeeReceiptAllocationResponse>()
        };
    }
}