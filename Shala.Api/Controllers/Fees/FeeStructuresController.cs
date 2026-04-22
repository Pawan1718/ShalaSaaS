using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.Fees;
using Shala.Domain.Entities.Fees;
using Shala.Domain.Enums;
using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Api.Controllers.Fees;

[Route("api/fees/structures")]
public class FeeStructuresController : TenantApiControllerBase
{
    private readonly IFeeStructureService _service;

    public FeeStructuresController(
        IFeeStructureService service,
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

        return Ok(new
        {
            success = true,
            message = "Fee structures fetched successfully.",
            data = data.Select(MapResponse).ToList()
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var data = await _service.GetWithItemsAsync(TenantId, BranchId, id, cancellationToken);

        if (data is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Fee structure not found.",
                data = (FeeStructureResponse?)null
            });
        }

        return Ok(new
        {
            success = true,
            message = "Fee structure fetched successfully.",
            data = MapResponse(data)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFeeStructureRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new FeeStructure
        {
            AcademicYearId = request.AcademicYearId,
            AcademicClassId = request.AcademicClassId,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            Items = request.Items.Select(x => new FeeStructureItem
            {
                FeeHeadId = x.FeeHeadId,
                Label = x.Label,
                Amount = x.Amount,
                FrequencyType = (FeeFrequencyType)x.FrequencyType,
                StartMonth = x.StartMonth,
                EndMonth = x.EndMonth,
                DueDay = x.DueDay,
                ApplyType = (FeeApplyType)x.ApplyType,
                IsOptional = x.IsOptional,
                IsActive = x.IsActive
            }).ToList()
        };

        var result = await _service.CreateAsync(TenantId, BranchId, Actor, entity, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Message,
                data = (FeeStructureResponse?)null
            });
        }

        return Ok(new
        {
            success = true,
            message = result.Message,
            data = MapResponse(result.Data!)
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateFeeStructureRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new FeeStructure
        {
            Id = id,
            AcademicYearId = request.AcademicYearId,
            AcademicClassId = request.AcademicClassId,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            Items = request.Items.Select(x => new FeeStructureItem
            {
                FeeHeadId = x.FeeHeadId,
                Label = x.Label,
                Amount = x.Amount,
                FrequencyType = (FeeFrequencyType)x.FrequencyType,
                StartMonth = x.StartMonth,
                EndMonth = x.EndMonth,
                DueDay = x.DueDay,
                ApplyType = (FeeApplyType)x.ApplyType,
                IsOptional = x.IsOptional,
                IsActive = x.IsActive
            }).ToList()
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

    private static FeeStructureResponse MapResponse(FeeStructure x)
    {
        return new FeeStructureResponse
        {
            Id = x.Id,
            BranchId = x.BranchId,
            AcademicYearId = x.AcademicYearId,
            AcademicClassId = x.AcademicClassId,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Items = x.Items.Select(i => new FeeStructureItemResponse
            {
                Id = i.Id,
                FeeHeadId = i.FeeHeadId,
                Label = i.Label,
                Amount = i.Amount,
                FrequencyType = (int)i.FrequencyType,
                StartMonth = i.StartMonth,
                EndMonth = i.EndMonth,
                DueDay = i.DueDay,
                ApplyType = (int)i.ApplyType,
                IsOptional = i.IsOptional,
                IsActive = i.IsActive
            }).ToList()
        };
    }
}