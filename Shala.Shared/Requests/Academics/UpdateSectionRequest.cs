using Shala.Shared.Requests.Students;

namespace Shala.Shared.Requests.Academics;

public class UpdateSectionRequest : CreateSectionRequest
{
    public int Id { get; set; }
}