using Shala.Application.Common;
using Shala.Application.Repositories.Fees;
using Shala.Domain.Entities.Fees;

namespace Shala.Application.Features.Fees;

public sealed class FeeReceiptNumberGenerator : IFeeReceiptNumberGenerator
{
    private readonly IFeeReceiptCounterRepository _counterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FeeReceiptNumberGenerator(
        IFeeReceiptCounterRepository counterRepository,
        IUnitOfWork unitOfWork)
    {
        _counterRepository = counterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GenerateAsync(
        int tenantId,
        int branchId,
        CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;

        var counter = await _counterRepository.GetAsync(
            tenantId,
            branchId,
            year,
            cancellationToken);

        if (counter is null)
        {
            counter = new FeeReceiptCounter
            {
                TenantId = tenantId,
                BranchId = branchId,
                Year = year,
                LastNumber = 1
            };

            await _counterRepository.AddAsync(counter, cancellationToken);
        }
        else
        {
            counter.LastNumber++;
            _counterRepository.Update(counter);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return $"RCPT-{year}-{counter.LastNumber:D4}";
    }
}