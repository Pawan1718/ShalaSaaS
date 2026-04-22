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
    [Route("api/registration/prospectus-configuration")]
    public sealed class RegistrationProspectusConfigurationController : TenantApiControllerBase
    {
        private readonly IRegistrationProspectusConfigurationService _service;

        public RegistrationProspectusConfigurationController(
            IRegistrationProspectusConfigurationService service,
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
            : base(currentUser, accessScopeValidator)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ApiResponse<RegistrationProspectusConfigurationResponse?>> GetAsync(
            CancellationToken cancellationToken)
        {
            var result = await _service.GetAsync(
                TenantId,
                BranchId,
                cancellationToken);

            return ApiResponse<RegistrationProspectusConfigurationResponse?>.Ok(result);
        }

        [HttpPost]
        public async Task<ApiResponse<RegistrationProspectusConfigurationResponse>> SaveAsync(
            [FromBody] SaveRegistrationProspectusConfigurationRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<RegistrationProspectusConfigurationResponse>.Fail("Request body is required.");

            var result = await _service.SaveAsync(
                TenantId,
                BranchId,
                request,
                cancellationToken);

            return ApiResponse<RegistrationProspectusConfigurationResponse>.Ok(
                result,
                "Registration prospectus configuration saved successfully.");
        }
    }
}