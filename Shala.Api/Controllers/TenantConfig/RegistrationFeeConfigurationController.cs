using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.TenantConfig;
using Shala.Shared.Common;
using Shala.Shared.Requests.TenantConfigSetting;
using Shala.Shared.Responses.TenantConfigSetting;

namespace Shala.Api.Controllers.TenantConfig
{
    [ApiController]
    [Route("api/registration/fee-configuration")]
    public sealed class RegistrationFeeConfigurationController : TenantApiControllerBase
    {
        private readonly IRegistrationFeeConfigurationService _service;

        public RegistrationFeeConfigurationController(
            IRegistrationFeeConfigurationService service,
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
            : base(currentUser, accessScopeValidator)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ApiResponse<RegistrationFeeConfigurationResponse?>> GetAsync(
            CancellationToken cancellationToken)
        {
            var result = await _service.GetAsync(
                TenantId,
                BranchId,
                cancellationToken);

            return ApiResponse<RegistrationFeeConfigurationResponse?>.Ok(result);
        }
        [HttpPost]
        public async Task<ApiResponse<RegistrationFeeConfigurationResponse>> SaveAsync(
            [FromBody] SaveRegistrationFeeConfigurationRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<RegistrationFeeConfigurationResponse>.Fail("Request body is required.");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                return ApiResponse<RegistrationFeeConfigurationResponse>.Fail(
                    string.IsNullOrWhiteSpace(errors) ? "Invalid request." : errors);
            }

            var result = await _service.SaveAsync(
                TenantId,
                BranchId,
                request,
                cancellationToken);

            return ApiResponse<RegistrationFeeConfigurationResponse>.Ok(
                result,
                "Registration fee configuration saved successfully.");
        }
    }
}