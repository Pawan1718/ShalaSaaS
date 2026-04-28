using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.Settings;
using Shala.Shared.Common;
using Shala.Shared.Requests.Settings;
using Shala.Shared.Responses.Settings;

namespace Shala.Api.Controllers.Settings
{
    [ApiController]
    [Route("api/settings/branch-document-profile")]
    public sealed class BranchDocumentProfileController : TenantApiControllerBase
    {
        private readonly IBranchDocumentProfileService _service;

        public BranchDocumentProfileController(
            IBranchDocumentProfileService service,
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
            : base(currentUser, accessScopeValidator)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ApiResponse<BranchDocumentProfileResponse?>> GetAsync(
            CancellationToken cancellationToken)
        {
            var branchId = await GetSafeBranchIdAsync(null, cancellationToken);
            var result = await _service.GetAsync(TenantId, branchId, cancellationToken);
            return ApiResponse<BranchDocumentProfileResponse?>.Ok(result);
        }

        [HttpPost]
        public async Task<ApiResponse<BranchDocumentProfileResponse>> SaveAsync(
            [FromBody] SaveBranchDocumentProfileRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<BranchDocumentProfileResponse>.Fail("Request body is required.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                return ApiResponse<BranchDocumentProfileResponse>.Fail(
                    string.IsNullOrWhiteSpace(errors) ? "Invalid request." : errors);
            }

            var branchId = await GetSafeBranchIdAsync(null, cancellationToken);
            var result = await _service.SaveAsync(TenantId, branchId, request, cancellationToken);

            return ApiResponse<BranchDocumentProfileResponse>.Ok(
                result,
                "Branch document profile saved successfully.");
        }
    }
}