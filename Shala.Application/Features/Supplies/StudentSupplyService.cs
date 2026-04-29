using Shala.Application.Common;
using Shala.Application.Repositories.Supplies;
using Shala.Domain.Entities.Supplies;
using Shala.Domain.Enums;
using Shala.Shared.Requests.Supplies;
using Shala.Shared.Responses.Supplies;

namespace Shala.Application.Features.Supplies;

public class StudentSupplyService : IStudentSupplyService
{
    private readonly ISupplyItemRepository _itemRepository;
    private readonly IStudentSupplyIssueRepository _issueRepository;
    private readonly IStudentSupplyPaymentRepository _paymentRepository;
    private readonly ISupplyStockLedgerRepository _stockLedgerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentSupplyService(
        ISupplyItemRepository itemRepository,
        IStudentSupplyIssueRepository issueRepository,
        IStudentSupplyPaymentRepository paymentRepository,
        ISupplyStockLedgerRepository stockLedgerRepository,
        IUnitOfWork unitOfWork)
    {
        _itemRepository = itemRepository;
        _issueRepository = issueRepository;
        _paymentRepository = paymentRepository;
        _stockLedgerRepository = stockLedgerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SupplyItemResponse>> GetItemsAsync(int tenantId, int branchId, bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var items = await _itemRepository.GetAllAsync(tenantId, branchId, activeOnly, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<(bool Success, string Message, SupplyItemResponse? Data)> CreateItemAsync(int tenantId, int branchId, string actor, CreateSupplyItemRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, "Item name is required.", null);

        if (string.IsNullOrWhiteSpace(request.Code))
            return (false, "Item code is required.", null);

        if (request.SalePrice < 0)
            return (false, "Sale price cannot be negative.", null);

        if (request.OpeningStock < 0)
            return (false, "Opening stock cannot be negative.", null);

        if (request.MinimumStock < 0)
            return (false, "Minimum stock cannot be negative.", null);

        var code = request.Code.Trim().ToUpperInvariant();

        var duplicate = await _itemRepository.GetByCodeAsync(code, tenantId, branchId, cancellationToken);
        if (duplicate is not null)
            return (false, "Item code already exists.", null);

        var now = DateTime.UtcNow;

        var item = new SupplyItem
        {
            TenantId = tenantId,
            BranchId = branchId,
            Name = request.Name.Trim(),
            Code = code,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            SalePrice = request.SalePrice,
            CurrentStock = request.OpeningStock,
            MinimumStock = request.MinimumStock,
            IsActive = request.IsActive,
            CreatedAt = now,
            CreatedBy = actor
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await _itemRepository.AddAsync(item, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.OpeningStock > 0)
            {
                await _stockLedgerRepository.AddAsync(new SupplyStockLedger
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    SupplyItemId = item.Id,
                    MovementDate = now,
                    MovementType = SupplyMovementType.In,
                    Quantity = request.OpeningStock,
                    BalanceAfter = item.CurrentStock,
                    ReferenceType = "OpeningStock",
                    ReferenceId = item.Id,
                    Remarks = "Opening stock",
                    CreatedAt = now,
                    CreatedBy = actor
                }, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return (true, "Supply item created successfully.", item.ToResponse());
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<(bool Success, string Message)> UpdateItemAsync(int tenantId, int branchId, string actor, int id, UpdateSupplyItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(id, tenantId, branchId, cancellationToken);
        if (item is null)
            return (false, "Supply item not found.");

        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, "Item name is required.");

        if (string.IsNullOrWhiteSpace(request.Code))
            return (false, "Item code is required.");

        if (request.SalePrice < 0)
            return (false, "Sale price cannot be negative.");

        if (request.MinimumStock < 0)
            return (false, "Minimum stock cannot be negative.");

        var code = request.Code.Trim().ToUpperInvariant();

        var duplicate = await _itemRepository.GetByCodeAsync(code, tenantId, branchId, cancellationToken);
        if (duplicate is not null && duplicate.Id != id)
            return (false, "Item code already exists.");

        item.Name = request.Name.Trim();
        item.Code = code;
        item.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        item.SalePrice = request.SalePrice;
        item.MinimumStock = request.MinimumStock;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = actor;

        _itemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Supply item updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteItemAsync(int tenantId, int branchId, int id, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(id, tenantId, branchId, cancellationToken);
        if (item is null)
            return (false, "Supply item not found.");

        var inUse = await _itemRepository.IsInUseAsync(id, tenantId, branchId, cancellationToken);
        if (inUse)
            return (false, "Supply item is already in use and cannot be deleted.");

        _itemRepository.Delete(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (true, "Supply item deleted successfully.");
    }

    public async Task<(bool Success, string Message)> AddStockAsync(int tenantId, int branchId, string actor, AddSupplyStockRequest request, CancellationToken cancellationToken = default)
    {
        if (request.SupplyItemId <= 0)
            return (false, "Item is required.");

        if (request.Quantity <= 0)
            return (false, "Quantity must be greater than zero.");

        var item = await _itemRepository.GetByIdAsync(request.SupplyItemId, tenantId, branchId, cancellationToken);
        if (item is null)
            return (false, "Supply item not found.");

        var now = DateTime.UtcNow;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            item.CurrentStock += request.Quantity;
            item.UpdatedAt = now;
            item.UpdatedBy = actor;

            _itemRepository.Update(item);

            await _stockLedgerRepository.AddAsync(new SupplyStockLedger
            {
                TenantId = tenantId,
                BranchId = branchId,
                SupplyItemId = item.Id,
                MovementDate = now,
                MovementType = request.MovementType == SupplyMovementType.Correction ? SupplyMovementType.Correction : SupplyMovementType.In,
                Quantity = request.Quantity,
                BalanceAfter = item.CurrentStock,
                ReferenceType = request.MovementType == SupplyMovementType.Correction ? "Correction" : "StockIn",
                ReferenceId = item.Id,
                Remarks = request.Remarks,
                CreatedAt = now,
                CreatedBy = actor
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return (true, "Stock added successfully.");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<(bool Success, string Message, StudentSupplyIssueResponse? Data)> CreateIssueAsync(int tenantId, int branchId, string actor, CreateStudentSupplyIssueRequest request, CancellationToken cancellationToken = default)
    {
        if (request.AcademicYearId <= 0)
            return (false, "Academic year is required.", null);

        if (request.StudentId <= 0)
            return (false, "Student is required.", null);

        if (request.StudentAdmissionId <= 0)
            return (false, "Student admission is required.", null);

        if (request.Items.Count == 0)
            return (false, "At least one item is required.", null);

        if (request.PaidAmount < 0)
            return (false, "Paid amount cannot be negative.", null);

        var requestedItemIds = request.Items.Select(x => x.SupplyItemId).Distinct().ToList();

        var items = await _itemRepository.GetByIdsAsync(requestedItemIds, tenantId, branchId, cancellationToken);

        if (items.Count != requestedItemIds.Count)
            return (false, "One or more supply items were not found.", null);

        var itemMap = items.ToDictionary(x => x.Id);
        var now = DateTime.UtcNow;
        var issueItems = new List<StudentSupplyIssueItem>();

        foreach (var row in request.Items)
        {
            if (row.Quantity <= 0)
                return (false, "Quantity must be greater than zero.", null);

            var item = itemMap[row.SupplyItemId];

            if (!item.IsActive)
                return (false, $"{item.Name} is inactive.", null);

            if (item.CurrentStock < row.Quantity)
                return (false, $"{item.Name} stock available: {item.CurrentStock}, selected: {row.Quantity}.", null);

            var unitPrice = row.UnitPrice ?? item.SalePrice;

            if (unitPrice < 0)
                return (false, "Unit price cannot be negative.", null);

            issueItems.Add(new StudentSupplyIssueItem
            {
                SupplyItemId = item.Id,
                ItemName = item.Name,
                ItemCode = item.Code,
                Quantity = row.Quantity,
                UnitPrice = unitPrice,
                LineTotal = row.Quantity * unitPrice,
                CreatedAt = now,
                CreatedBy = actor
            });
        }

        var totalAmount = issueItems.Sum(x => x.LineTotal);

        if (request.PaidAmount > totalAmount)
            return (false, "Paid amount cannot be greater than total amount.", null);

        var issue = new StudentSupplyIssue
        {
            TenantId = tenantId,
            BranchId = branchId,
            AcademicYearId = request.AcademicYearId,
            StudentId = request.StudentId,
            StudentAdmissionId = request.StudentAdmissionId,
            IssueNo = await _issueRepository.GenerateNextIssueNoAsync(tenantId, branchId, cancellationToken),
            IssueDate = now,
            TotalAmount = totalAmount,
            PaidAmount = request.PaidAmount,
            DueAmount = totalAmount - request.PaidAmount,
            PaymentStatus = GetPaymentStatus(totalAmount, request.PaidAmount),
            Remarks = request.Remarks,
            CreatedAt = now,
            CreatedBy = actor,
            Items = issueItems
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await _issueRepository.AddAsync(issue, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var issueItem in issueItems)
            {
                var item = itemMap[issueItem.SupplyItemId];

                item.CurrentStock -= issueItem.Quantity;

                if (item.CurrentStock < 0)
                    return (false, "Stock cannot go negative.", null);

                item.UpdatedAt = now;
                item.UpdatedBy = actor;

                _itemRepository.Update(item);

                await _stockLedgerRepository.AddAsync(new SupplyStockLedger
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    SupplyItemId = item.Id,
                    MovementDate = now,
                    MovementType = SupplyMovementType.Out,
                    Quantity = issueItem.Quantity,
                    BalanceAfter = item.CurrentStock,
                    ReferenceType = "SupplyIssue",
                    ReferenceId = issue.Id,
                    Remarks = issue.IssueNo,
                    CreatedAt = now,
                    CreatedBy = actor
                }, cancellationToken);
            }

            if (request.PaidAmount > 0)
            {
                await _paymentRepository.AddAsync(new StudentSupplyPayment
                {
                    TenantId = tenantId,
                    BranchId = branchId,
                    AcademicYearId = request.AcademicYearId,
                    StudentSupplyIssueId = issue.Id,
                    PaymentDate = now,
                    Amount = request.PaidAmount,
                    PaymentMode = request.PaymentMode,
                    ReferenceNo = request.ReferenceNo,
                    Remarks = request.Remarks,
                    CreatedAt = now,
                    CreatedBy = actor
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var loadedIssue = await _issueRepository.GetDetailAsync(issue.Id, tenantId, branchId, cancellationToken);

            return (true, "Supplies issued successfully.", loadedIssue?.ToResponse() ?? issue.ToResponse());
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<StudentSupplyIssueResponse?> GetIssueDetailAsync(int tenantId, int branchId, int id, CancellationToken cancellationToken = default)
    {
        var issue = await _issueRepository.GetDetailAsync(id, tenantId, branchId, cancellationToken);
        return issue?.ToResponse();
    }

    public async Task<List<StudentSupplyIssueResponse>> GetRecentIssuesAsync(int tenantId, int branchId, int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var issues = await _issueRepository.GetRecentAsync(tenantId, branchId, academicYearId, cancellationToken);
        return issues.Select(x => x.ToResponse()).ToList();
    }

    public async Task<List<StudentSupplyIssueResponse>> GetStudentHistoryAsync(int tenantId, int branchId, int studentId, int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var issues = await _issueRepository.GetStudentHistoryAsync(tenantId, branchId, studentId, academicYearId, cancellationToken);
        return issues.Select(x => x.ToResponse()).ToList();
    }

    public async Task<List<PendingSupplyDueResponse>> GetPendingDuesAsync(int tenantId, int branchId, int? academicYearId = null, int? studentId = null, CancellationToken cancellationToken = default)
    {
        var dues = await _issueRepository.GetPendingDuesAsync(tenantId, branchId, academicYearId, studentId, cancellationToken);

        return dues.Select(x => new PendingSupplyDueResponse
        {
            IssueId = x.Id,
            IssueNo = x.IssueNo,
            IssueDate = x.IssueDate,
            StudentId = x.StudentId,
            StudentName = x.Student is null ? string.Empty : $"{x.Student.FirstName} {x.Student.LastName}".Trim(),
            AdmissionNo = x.StudentAdmission?.AdmissionNo ?? string.Empty,
            TotalAmount = x.TotalAmount,
            PaidAmount = x.PaidAmount,
            DueAmount = x.DueAmount
        }).ToList();
    }

    public async Task<(bool Success, string Message, StudentSupplyIssueResponse? Data)> CollectDueAsync(int tenantId, int branchId, string actor, int issueId, CollectSupplyDueRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0)
            return (false, "Amount must be greater than zero.", null);

        var issue = await _issueRepository.GetDetailAsync(issueId, tenantId, branchId, cancellationToken);

        if (issue is null)
            return (false, "Supply bill not found.", null);

        if (issue.IsCancelled)
            return (false, "Cancelled bill cannot accept payment.", null);

        if (request.Amount > issue.DueAmount)
            return (false, "Payment amount cannot be greater than due amount.", null);

        var now = DateTime.UtcNow;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            issue.PaidAmount += request.Amount;
            issue.DueAmount = issue.TotalAmount - issue.PaidAmount;
            issue.PaymentStatus = GetPaymentStatus(issue.TotalAmount, issue.PaidAmount);
            issue.UpdatedAt = now;
            issue.UpdatedBy = actor;

            _issueRepository.Update(issue);

            await _paymentRepository.AddAsync(new StudentSupplyPayment
            {
                TenantId = tenantId,
                BranchId = branchId,
                AcademicYearId = issue.AcademicYearId,
                StudentSupplyIssueId = issue.Id,
                PaymentDate = now,
                Amount = request.Amount,
                PaymentMode = request.PaymentMode,
                ReferenceNo = request.ReferenceNo,
                Remarks = request.Remarks,
                CreatedAt = now,
                CreatedBy = actor
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var loadedIssue = await _issueRepository.GetDetailAsync(issue.Id, tenantId, branchId, cancellationToken);

            return (true, "Payment collected successfully.", loadedIssue?.ToResponse() ?? issue.ToResponse());
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<SupplyDashboardResponse> GetDashboardAsync(int tenantId, int branchId, int? academicYearId = null, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        return new SupplyDashboardResponse
        {
            TodayIssueCount = await _issueRepository.GetTodayIssueCountAsync(tenantId, branchId, today, cancellationToken),
            TodayCollection = await _paymentRepository.GetCollectionTotalAsync(tenantId, branchId, today, today.AddDays(1), academicYearId, cancellationToken),
            PendingDue = await _issueRepository.GetPendingDueTotalAsync(tenantId, branchId, academicYearId, cancellationToken),
            LowStockItemCount = (await _itemRepository.GetLowStockAsync(tenantId, branchId, cancellationToken)).Count
        };
    }

    public async Task<List<SupplyItemResponse>> GetLowStockAsync(int tenantId, int branchId, CancellationToken cancellationToken = default)
    {
        var items = await _itemRepository.GetLowStockAsync(tenantId, branchId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<List<SupplyStockLedgerResponse>> GetStockHistoryAsync(int tenantId, int branchId, SupplyReportRequest request, CancellationToken cancellationToken = default)
    {
        var ledgers = await _stockLedgerRepository.GetHistoryAsync(
            tenantId,
            branchId,
            request.FromDate,
            request.ToDate,
            request.SupplyItemId,
            cancellationToken);

        return ledgers.Select(x => x.ToResponse()).ToList();
    }

    private static SupplyPaymentStatus GetPaymentStatus(decimal totalAmount, decimal paidAmount)
    {
        if (paidAmount <= 0)
            return SupplyPaymentStatus.Due;

        if (paidAmount >= totalAmount)
            return SupplyPaymentStatus.Paid;

        return SupplyPaymentStatus.Partial;
    }
}