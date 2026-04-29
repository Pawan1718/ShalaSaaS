using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Api.Controllers.Fees;

[Route("api/fees/heads")]
public class FeeHeadsController : TenantApiControllerBase
{
    private readonly IFeeHeadService _service;

    public FeeHeadsController(
        IFeeHeadService service,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await _service.GetAllAsync(TenantId, BranchId, cancellationToken);

        var result = data.Select(MapResponse).ToList();

        return Ok(new
        {
            success = true,
            message = "Fee heads fetched successfully.",
            data = result
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var data = await _service.GetByIdAsync(TenantId, BranchId, id, cancellationToken);

        if (data is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Fee head not found.",
                data = (FeeHeadResponse?)null
            });
        }

        return Ok(new
        {
            success = true,
            message = "Fee head fetched successfully.",
            data = MapResponse(data)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFeeHeadRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new FeeHead
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            IsRegistrationFee = request.IsRegistrationFee,
            IsActive = request.IsActive
        };

        var result = await _service.CreateAsync(TenantId, BranchId, Actor, entity, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Message,
                data = (FeeHeadResponse?)null
            });
        }

        var response = MapResponse(result.Data!);

        return Ok(new
        {
            success = true,
            message = result.Message,
            data = response
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateFeeHeadRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new FeeHead
        {
            Id = id,
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            IsRegistrationFee = request.IsRegistrationFee,
            IsActive = request.IsActive
        };

        var result = await _service.UpdateAsync(TenantId, BranchId, Actor, entity, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Message,
                data = false
            });
        }

        return Ok(new
        {
            success = true,
            message = result.Message,
            data = true
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(TenantId, BranchId, id, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Message,
                data = false
            });
        }

        return Ok(new
        {
            success = true,
            message = result.Message,
            data = true
        });
    }

    private static FeeHeadResponse MapResponse(FeeHead x)
    {
        return new FeeHeadResponse
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Description = x.Description,
            IsRegistrationFee = x.IsRegistrationFee,
            IsActive = x.IsActive
        };
    }
}