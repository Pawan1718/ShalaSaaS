using Shala.Shared.Requests.Academics;

namespace Shala.Shared.Requests.Students;

public class UpdateStudentAdmissionRequest : CreateStudentAdmissionRequest
{
    public int Id { get; set; }
}