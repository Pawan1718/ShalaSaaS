using Shala.Shared.Common;
using Shala.Shared.Requests.Academics;
using Shala.Shared.Requests.Students;
using Shala.Shared.Responses.Academics;
using Shala.Shared.Responses.Students;
using Shala.Web.Repositories.Base;
using Shala.Web.Repositories.Interfaces;
using Shala.Web.Services.State;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Shala.Web.Repositories.Students;

public class StudentRepository : RepositoryBase, IStudentRepository
{
    public StudentRepository(HttpClient httpClient, ApiSession session)
        : base(httpClient, session)
    {
    }

    public async Task<ApiResponse<PagedResult<StudentListItemResponse>>?> GetPagedAsync(StudentListRequest request)
    {
        await EnsureAuthAsync();
        var response = await HttpClient.PostAsJsonAsync("api/students/search", request);
        return await ReadApiResponse<ApiResponse<PagedResult<StudentListItemResponse>>>(response, "Failed to load students.");
    }

    public async Task<ApiResponse<StudentDetailsResponse>?> GetByIdAsync(int id, int tenantId)
    {
        await EnsureAuthAsync();
        var response = await HttpClient.GetAsync($"api/students/{id}?tenantId={tenantId}");
        return await ReadApiResponse<ApiResponse<StudentDetailsResponse>>(response, "Failed to load student details.");
    }

    public async Task<ApiResponse<StudentDetailsResponse>?> CreateAsync(CreateStudentRequest request)
    {
        await EnsureAuthAsync();
        var response = await HttpClient.PostAsJsonAsync("api/students", request);
        return await ReadApiResponse<ApiResponse<StudentDetailsResponse>>(response, "Failed to create student.");
    }

    public async Task<ApiResponse<StudentDetailsResponse>?> UpdateAsync(int id, UpdateStudentRequest request)
    {
        await EnsureAuthAsync();
        var response = await HttpClient.PutAsJsonAsync($"api/students/{id}", request);
        return await ReadApiResponse<ApiResponse<StudentDetailsResponse>>(response, "Failed to update student.");
    }



    public async Task<ApiResponse<List<StudentAdmissionListItemResponse>>?> GetAdmissionsByAcademicYearAndClassAsync(
    int tenantId,
    int academicYearId,
    int classId)
    {
        await EnsureAuthAsync();

        var response = await HttpClient.GetAsync(
            $"api/students/admissions/by-academic-year-class?tenantId={tenantId}&academicYearId={academicYearId}&classId={classId}");

        return await ReadApiResponse<ApiResponse<List<StudentAdmissionListItemResponse>>>(
            response,
            "Failed to load admissions by academic year and class.");
    }

}