using Shala.Shared.Requests.Students;

namespace Shala.Shared.Requests.Academics;

public class UpdateAcademicYearRequest : CreateAcademicYearRequest
{
    public int Id { get; set; }
}