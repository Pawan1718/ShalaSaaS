using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.Registration;
using Shala.Application.Repositories.TenantConfig;
using Shala.Shared.Common;
using Shala.Shared.Requests.Registration;
using Shala.Shared.Responses.Registration;

namespace Shala.Api.Controllers.Registration
{
    [ApiController]
    [Route("api/registrations")]
    public sealed class RegistrationController : TenantApiControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IRegistrationFeeService _registrationFeeService;
        private readonly IRegistrationFeeConfigurationRepository _registrationConfigRepository;

        public RegistrationController(
            IRegistrationService registrationService,
            IRegistrationFeeService registrationFeeService,
            IRegistrationFeeConfigurationRepository registrationConfigRepository,
            ICurrentUserContext currentUser,
            IAccessScopeValidator accessScopeValidator)
            : base(currentUser, accessScopeValidator)
        {
            _registrationService = registrationService;
            _registrationFeeService = registrationFeeService;
            _registrationConfigRepository = registrationConfigRepository;
        }

        [HttpGet]
        public async Task<ApiResponse<List<RegistrationDto>>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            var result = await _registrationService.GetAllAsync(
                TenantId,
                BranchId,
                cancellationToken);

            return ApiResponse<List<RegistrationDto>>.Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ApiResponse<RegistrationDto>> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _registrationService.GetByIdAsync(
                TenantId,
                BranchId,
                id,
                cancellationToken);

            return ApiResponse<RegistrationDto>.Ok(result);
        }

        [HttpPost]
        public async Task<ApiResponse<int>> CreateAsync(
            [FromBody] CreateRegistrationRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<int>.Fail("Request body is required.");

            if (!await IsRegistrationModuleEnabledAsync(cancellationToken))
                return ApiResponse<int>.Fail("Registration module is inactive for this branch.");

            var id = await _registrationService.CreateAsync(
                TenantId,
                BranchId,
                request,
                cancellationToken);

            return ApiResponse<int>.Ok(id, "Registration created successfully.");
        }

        [HttpPost("paged")]
        public async Task<ApiResponse<PagedResult<RegistrationDto>>> GetPagedAsync(
            [FromBody] PagedRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<PagedResult<RegistrationDto>>.Fail("Request body is required.");

            var result = await _registrationService.GetPagedAsync(
                TenantId,
                BranchId,
                request,
                cancellationToken);

            return ApiResponse<PagedResult<RegistrationDto>>.Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ApiResponse<object?>> UpdateAsync(
            int id,
            [FromBody] UpdateRegistrationRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<object?>.Fail("Request body is required.");

            if (!await IsRegistrationModuleEnabledAsync(cancellationToken))
                return ApiResponse<object?>.Fail("Registration module is inactive for this branch.");

            await _registrationService.UpdateAsync(
                TenantId,
                BranchId,
                id,
                request,
                cancellationToken);

            return ApiResponse<object?>.Ok(null, "Registration updated successfully.");
        }

        [HttpDelete("{id:int}")]
        public async Task<ApiResponse<object?>> DeleteAsync(
            int id,
            CancellationToken cancellationToken)
        {
            if (!await IsRegistrationModuleEnabledAsync(cancellationToken))
                return ApiResponse<object?>.Fail("Registration module is inactive for this branch.");

            await _registrationService.DeleteAsync(
                TenantId,
                BranchId,
                id,
                cancellationToken);

            return ApiResponse<object?>.Ok(null, "Registration deleted successfully.");
        }

        [HttpPost("{id:int}/convert")]
        public async Task<ApiResponse<ConvertRegistrationResponse>> ConvertAsync(
            int id,
            [FromBody] ConvertRegistrationRequest? request,
            CancellationToken cancellationToken)
        {
            if (!await IsRegistrationModuleEnabledAsync(cancellationToken))
                return ApiResponse<ConvertRegistrationResponse>.Fail("Registration module is inactive for this branch.");

            request ??= new ConvertRegistrationRequest();

            var result = await _registrationService.ConvertAsync(
                TenantId,
                BranchId,
                id,
                request,
                Actor,
                cancellationToken);

            return ApiResponse<ConvertRegistrationResponse>.Ok(result, "Registration converted successfully.");
        }

        [HttpPost("save-with-fee")]
        public async Task<ApiResponse<RegistrationFeeResponse>> SaveWithFeeAsync(
            [FromBody] SaveRegistrationWithFeeRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<RegistrationFeeResponse>.Fail("Request body is required.");

            if (!await IsRegistrationModuleEnabledAsync(cancellationToken))
                return ApiResponse<RegistrationFeeResponse>.Fail("Registration module is inactive for this branch.");

            var result = await _registrationService.SaveWithFeeAsync(
                TenantId,
                BranchId,
                request,
                cancellationToken);

            return ApiResponse<RegistrationFeeResponse>.Ok(
                result,
                "Registration and fee saved successfully.");
        }

        [HttpPost("{id:int}/collect-fee")]
        public async Task<ApiResponse<RegistrationFeeResponse>> CollectFeeAsync(
            int id,
            [FromBody] CollectRegistrationFeeRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<RegistrationFeeResponse>.Fail("Request body is required.");

            if (!await IsRegistrationModuleEnabledAsync(cancellationToken))
                return ApiResponse<RegistrationFeeResponse>.Fail("Registration module is inactive for this branch.");

            var result = await _registrationFeeService.CollectAsync(
                TenantId,
                BranchId,
                id,
                request,
                cancellationToken);

            return ApiResponse<RegistrationFeeResponse>.Ok(
                result,
                "Registration fee collected successfully.");
        }

        [HttpGet("receipt/{receiptId:int}")]
        public async Task<ApiResponse<RegistrationReceiptResponse>> GetReceiptAsync(
            int receiptId,
            CancellationToken cancellationToken)
        {
            var result = await _registrationFeeService.GetReceiptAsync(
                TenantId,
                BranchId,
                receiptId,
                cancellationToken);

            return ApiResponse<RegistrationReceiptResponse>.Ok(result);
        }


        [HttpPost("receipt/{receiptId:int}/cancel")]
        public async Task<ApiResponse<object?>> CancelReceiptAsync(
    int receiptId,
    [FromBody] CancelRegistrationReceiptRequest request,
    CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<object?>.Fail("Request body is required.");

            await _registrationFeeService.CancelReceiptAsync(
                TenantId,
                BranchId,
                receiptId,
                Actor,
                request,
                cancellationToken);

            return ApiResponse<object?>.Ok(null, "Receipt cancelled successfully.");
        }


        [HttpPost("receipt/{receiptId:int}/refund")]
        public async Task<ApiResponse<object?>> RefundReceiptAsync(
    int receiptId,
    [FromBody] RefundRegistrationReceiptRequest request,
    CancellationToken cancellationToken)
        {
            if (request is null)
                return ApiResponse<object?>.Fail("Request body is required.");

            await _registrationFeeService.RefundReceiptAsync(
                TenantId,
                BranchId,
                receiptId,
                Actor,
                request,
                cancellationToken);

            return ApiResponse<object?>.Ok(null, "Receipt refunded successfully.");
        }


        private async Task<bool> IsRegistrationModuleEnabledAsync(CancellationToken cancellationToken)
        {
            var config = await _registrationConfigRepository.GetByScopeAsync(
                TenantId,
                BranchId,
                cancellationToken);

            return config?.IsRegistrationModuleEnabled ?? true;
        }
    }
}