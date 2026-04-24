using Shala.Domain.Entities.Supplies;
using Shala.Shared.Common;
using Shala.Shared.Responses.Supplies;

namespace Shala.Application.Features.Supplies;

public static class SupplyMappings
{
    public static SupplyItemResponse ToResponse(this SupplyItem item)
    {
        return new SupplyItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Description = item.Description,
            SalePrice = item.SalePrice,
            CurrentStock = item.CurrentStock,
            MinimumStock = item.MinimumStock,
            IsActive = item.IsActive
        };
    }

    public static StudentSupplyIssueItemResponse ToResponse(this StudentSupplyIssueItem item)
    {
        return new StudentSupplyIssueItemResponse
        {
            Id = item.Id,
            SupplyItemId = item.SupplyItemId,
            ItemName = item.ItemName,
            ItemCode = item.ItemCode,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal
        };
    }

    public static StudentSupplyPaymentResponse ToResponse(this StudentSupplyPayment payment)
    {
        return new StudentSupplyPaymentResponse
        {
            Id = payment.Id,
            PaymentDate = payment.PaymentDate,
            Amount = payment.Amount,
            PaymentMode = payment.PaymentMode,
            ReferenceNo = payment.ReferenceNo,
            Remarks = payment.Remarks
        };
    }

    public static StudentSupplyIssueResponse ToResponse(this StudentSupplyIssue issue)
    {
        return new StudentSupplyIssueResponse
        {
            Id = issue.Id,
            AcademicYearId = issue.AcademicYearId,
            StudentId = issue.StudentId,
            StudentAdmissionId = issue.StudentAdmissionId,
            IssueNo = issue.IssueNo,
            IssueDate = issue.IssueDate,

            StudentName = issue.Student is null
                ? string.Empty
                : $"{issue.Student.FirstName} {issue.Student.LastName}".Trim(),

            AdmissionNo = issue.StudentAdmission?.AdmissionNo ?? string.Empty,
            ClassName = issue.StudentAdmission?.AcademicClass?.Name ?? string.Empty,
            SectionName = issue.StudentAdmission?.Section?.Name,

            TotalAmount = issue.TotalAmount,
            PaidAmount = issue.PaidAmount,
            DueAmount = issue.DueAmount,
            PaymentStatus = issue.PaymentStatus,
            Remarks = issue.Remarks,

            Items = issue.Items.Select(x => x.ToResponse()).ToList(),
            Payments = issue.Payments.Select(x => x.ToResponse()).ToList()
        };
    }

    public static SupplyStockLedgerResponse ToResponse(this SupplyStockLedger ledger)
    {
        return new SupplyStockLedgerResponse
        {
            Id = ledger.Id,
            SupplyItemId = ledger.SupplyItemId,
            ItemName = ledger.SupplyItem?.Name ?? string.Empty,
            MovementDate = ledger.MovementDate,
            MovementType = ledger.MovementType,
            Quantity = ledger.Quantity,
            BalanceAfter = ledger.BalanceAfter,
            ReferenceType = ledger.ReferenceType,
            ReferenceId = ledger.ReferenceId,
            Remarks = ledger.Remarks
        };
    }

    public static PagedResult<SupplyItemResponse> ToSupplyItemResponsePaged(
        this PagedResult<SupplyItem> result)
    {
        return new PagedResult<SupplyItemResponse>
        {
            Items = result.Items.Select(x => x.ToResponse()).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}