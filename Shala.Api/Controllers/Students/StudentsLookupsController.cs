using Microsoft.AspNetCore.Mvc;
using Shala.Api.Controllers;
using Shala.Application.Contracts;
using Shala.Application.Features.Academics;
using Shala.Shared.Common;
using Shala.Shared.Responses.Students;

namespace Shala.Api.Controllers.Students;

[Route("api/students/lookups")]
public class StudentsLookupsController : TenantApiControllerBase
{
    private readonly IAcademicYearService _academicYearService;
    private readonly IAcademicClassService _academicClassService;
    private readonly ISectionService _sectionService;

    public StudentsLookupsController(
        IAcademicYearService academicYearService,
        IAcademicClassService academicClassService,
        ISectionService sectionService,
        ICurrentUserContext currentUser,
        IAccessScopeValidator accessScopeValidator)
        : base(currentUser, accessScopeValidator)
    {
        _academicYearService = academicYearService;
        _academicClassService = academicClassService;
        _sectionService = sectionService;
    }

    [HttpGet("academic-years")]
    public async Task<ActionResult<ApiResponse<List<LookupItemResponse>>>> GetAcademicYears(
        CancellationToken cancellationToken)
    {
        var result = await _academicYearService.GetLookupAsync(TenantId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("classes")]
    public async Task<ActionResult<ApiResponse<List<LookupItemResponse>>>> GetClasses(
        CancellationToken cancellationToken)
    {
        var result = await _academicClassService.GetLookupAsync(TenantId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("sections")]
    public async Task<ActionResult<ApiResponse<List<LookupItemResponse>>>> GetSections(
        [FromQuery] int classId,
        CancellationToken cancellationToken)
    {
        var result = await _sectionService.GetLookupByClassAsync(
            TenantId,
            BranchId,
            classId,
            cancellationToken);

        return Ok(result);
    }
}