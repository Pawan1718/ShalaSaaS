using Shala.Application.Common;
using Shala.Domain.Entities.Supplies;

namespace Shala.Application.Repositories.Supplies;

public interface IStudentSupplyPaymentRepository : IGenericRepository<StudentSupplyPayment>
{
    Task<List<StudentSupplyPayment>> GetByIssueIdAsync(int issueId, int tenantId, int branchId, CancellationToken cancellationToken = default);

    Task<decimal> GetCollectionTotalAsync(int tenantId, int branchId, DateTime fromDate, DateTime toDate, int? academicYearId = null, CancellationToken cancellationToken = default);
}