using Shala.Application.Repositories.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Application.Common
{
    public class BranchCodeGenerator : IBranchCodeGenerator
    {
        private readonly IBranchRepository _branchRepository;

        public BranchCodeGenerator(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public async Task<string> GenerateAsync(
            int tenantId,
            string branchName,
            CancellationToken cancellationToken = default)
        {
            var baseCode = BuildBaseCode(branchName);

            var code = baseCode;
            var counter = 1;

            while (await _branchRepository.ExistsByCodeAsync(tenantId, code, null, cancellationToken))
            {
                counter++;
                code = $"{baseCode}{counter}";
            }

            return code;
        }

        private static string BuildBaseCode(string? branchName)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                return "BRANCH";

            var words = branchName
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            string code;

            if (words.Count >= 2)
            {
                code = string.Concat(words.Select(w => char.ToUpperInvariant(w[0])));
            }
            else
            {
                var cleaned = new string(branchName
                    .Where(char.IsLetterOrDigit)
                    .ToArray())
                    .ToUpperInvariant();

                code = cleaned.Length <= 6 ? cleaned : cleaned[..6];
            }

            return string.IsNullOrWhiteSpace(code) ? "BRANCH" : code;
        }
    }
}
