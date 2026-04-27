using Microsoft.AspNetCore.Mvc;
using Shala.Application.Contracts;
using Shala.Application.Features.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Common;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Api.Controllers.Fees;

[Route("api/fees/assignments")]
public class StudentFeeAssignmentsController : TenantApiControllerBase
{
    private readonly IStudentFeeAssignmentService _assignmentService;
    private readonly IFeeChargeGenerationService _chargeGenerationService;

    public StudentFeeAssignmentsController(
        IStudentFeeAssignmentService assignmentService,
        IFeeChargeGenerationService chargeGenerationService,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _assignmentService = assignmentService;
        _chargeGenerationService = chargeGenerationService;
    }

    [HttpPost]
    public async Task<IActionResult> Assign(
        [FromBody] CreateStudentFeeAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var entity = new StudentFeeAssignment
        {
            StudentId = request.StudentId,
            StudentAdmissionId = request.StudentAdmissionId,
            FeeStructureId = request.FeeStructureId,
            DiscountAmount = request.DiscountAmount,
            AdditionalChargeAmount = request.AdditionalChargeAmount,
            IsActive = request.IsActive
        };

        var result = await _assignmentService.AssignAsync(
            TenantId,
            branchId,
            entity,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new ApiResponse<StudentFeeAssignmentResponse?>
            {
                Success = false,
                Message = result.Message,
                Data = null
            });
        }

        if (result.Data is null)
        {
            return Ok(new ApiResponse<StudentFeeAssignmentResponse?>
            {
                Success = true,
                Message = result.Message,
                Data = null
            });
        }

        return Ok(new ApiResponse<StudentFeeAssignmentResponse>
        {
            Success = true,
            Message = result.Message,
            Data = MapAssignment(result.Data)
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateStudentFeeAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        if (id != request.Id)
        {
            return BadRequest(new ApiResponse<StudentFeeAssignmentResponse?>
            {
                Success = false,
                Message = "Route id and payload id do not match.",
                Data = null
            });
        }

        var entity = new StudentFeeAssignment
        {
            Id = request.Id,
            StudentId = request.StudentId,
            StudentAdmissionId = request.StudentAdmissionId,
            FeeStructureId = request.FeeStructureId,
            DiscountAmount = request.DiscountAmount,
            AdditionalChargeAmount = request.AdditionalChargeAmount,
            IsActive = request.IsActive
        };

        var result = await _assignmentService.UpdateAsync(
            TenantId,
            branchId,
            entity,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new ApiResponse<StudentFeeAssignmentResponse?>
            {
                Success = false,
                Message = result.Message,
                Data = null
            });
        }

        var updated = await _assignmentService.GetByIdAsync(
            TenantId,
            branchId,
            id,
            cancellationToken);

        if (updated is null)
        {
            return NotFound(new ApiResponse<StudentFeeAssignmentResponse?>
            {
                Success = false,
                Message = "Student fee assignment not found.",
                Data = null
            });
        }

        return Ok(new ApiResponse<StudentFeeAssignmentResponse>
        {
            Success = true,
            Message = result.Message,
            Data = MapAssignment(updated)
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _assignmentService.DeleteAsync(
            TenantId,
            branchId,
            id,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = result.Message,
                Data = false
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = result.Message,
            Data = true
        });
    }

    [HttpGet("{id:int}/can-modify")]
    public async Task<IActionResult> CanModify(
        int id,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _assignmentService.CanModifyAssignmentAsync(
            TenantId,
            branchId,
            id,
            cancellationToken);

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = result.Message,
            Data = result.CanModify
        });
    }

    [HttpGet("admission/{admissionId:int}")]
    public async Task<IActionResult> GetByAdmission(
        int admissionId,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _assignmentService.GetByAdmissionIdAsync(
            TenantId,
            branchId,
            admissionId,
            cancellationToken);

        return Ok(new ApiResponse<List<StudentFeeAssignmentResponse>>
        {
            Success = true,
            Message = "Success",
            Data = result.Select(MapAssignment).ToList()
        });
    }

    [HttpPost("{assignmentId:int}/generate-charges")]
    public async Task<IActionResult> GenerateCharges(
        int assignmentId,
        CancellationToken cancellationToken)
    {
        var branchId = await GetSafeBranchIdAsync(null, cancellationToken);

        var result = await _chargeGenerationService.GenerateAsync(
            TenantId,
            branchId,
            assignmentId,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new ApiResponse<List<StudentChargeResponse>>
            {
                Success = false,
                Message = result.Message,
                Data = new List<StudentChargeResponse>()
            });
        }

        return Ok(new ApiResponse<List<StudentChargeResponse>>
        {
            Success = true,
            Message = "Charges generated successfully.",
            Data = result.Charges.Select(MapCharge).ToList()
        });
    }

    private static StudentFeeAssignmentResponse MapAssignment(StudentFeeAssignment x)
    {
        return new StudentFeeAssignmentResponse
        {
            Id = x.Id,
            StudentId = x.StudentId,
            StudentAdmissionId = x.StudentAdmissionId,
            FeeStructureId = x.FeeStructureId,
            FeeStructureName = x.FeeStructure?.Name ?? "-",
            AcademicYear = x.FeeStructure?.AcademicYearId > 0
                ? $"Academic Year #{x.FeeStructure.AcademicYearId}"
                : "-",
            DiscountAmount = x.DiscountAmount,
            AdditionalChargeAmount = x.AdditionalChargeAmount,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        };
    }

    private static StudentChargeResponse MapCharge(StudentCharge x)
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