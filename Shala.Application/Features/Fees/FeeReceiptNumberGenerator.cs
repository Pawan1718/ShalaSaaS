using Shala.Application.Repositories.Fees;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Application.Features.Fees
{
    public class FeeReceiptNumberGenerator : IFeeReceiptNumberGenerator
    {
        private readonly IFeeReceiptRepository _repo;

        public FeeReceiptNumberGenerator(IFeeReceiptRepository repo)
        {
            _repo = repo;
        }

        public async Task<string> GenerateAsync(
            int tenantId,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            var receipts = await _repo.GetAllAsync(
                tenantId,
                branchId,
                cancellationToken);

            var count = receipts.Count + 1;

            return $"RCPT-{DateTime.UtcNow.Year}-{count:D4}";
        }
    }
}
