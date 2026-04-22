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
    [Route("api/registration/receipt-configuration")]
    public sealed class RegistrationReceiptConfigurationController : TenantApiControllerBase
    {
        private readonly IRegistrationReceiptConfigurationService _service;

        public RegistrationReceiptConfigurationController(
            IRegistrationReceiptConfigurationService service,
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
            : base(currentUser, accessScopeValidator)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ApiResponse<RegistrationReceiptConfigurationResponse?>> GetAsync(
            CancellationToken cancellationToken)
        {
            var result = await _service.GetAsync(
                TenantId,
                BranchId,
                cancellationToken);

            return ApiResponse<RegistrationReceiptConfigurationResponse?>.Ok(result);
        }

        [HttpPost]
        public async Task<ApiResponse<RegistrationReceiptConfigurationResponse>> SaveAsync(
            [FromBody] SaveRegistrationReceiptConfigurationRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<RegistrationReceiptConfigurationResponse>.Fail("Request body is required.");

            var result = await _service.SaveAsync(
                TenantId,
                BranchId,
                request,
                cancellationToken);

            return ApiResponse<RegistrationReceiptConfigurationResponse>.Ok(
                result,
                "Registration receipt configuration saved successfully.");
        }
    }
}