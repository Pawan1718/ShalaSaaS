using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Infrastructure.Data;
using Shala.Shared.Common;
using Shala.Shared.Responses.Registration;

namespace Shala.Api.Controllers.TenantConfig
{
    [ApiController]
    [Route("api/registration/fee-heads")]
    public sealed class RegistrationFeeHeadLookupController : TenantApiControllerBase
    {
        private readonly AppDbContext _dbContext;

        public RegistrationFeeHeadLookupController(
            AppDbContext dbContext,
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
            : base(currentUser, accessScopeValidator)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ApiResponse<List<RegistrationFeeHeadLookupResponse>>> GetAsync(
            CancellationToken cancellationToken)
        {
            var result = await _dbContext.FeeHeads
                .AsNoTracking()
                .Where(x =>
                    x.TenantId == TenantId &&
                    x.BranchId == BranchId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new RegistrationFeeHeadLookupResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<List<RegistrationFeeHeadLookupResponse>>.Ok(result);
        }
    }
}