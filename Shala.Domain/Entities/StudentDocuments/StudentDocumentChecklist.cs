using Shala.Domain.Common;
using Shala.Domain.Entities.Students;

namespace Shala.Domain.Entities.StudentDocuments;

public class StudentDocumentChecklist : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public int StudentAdmissionId { get; set; }
    public int DocumentModelId { get; set; }

    public bool IsReceived { get; set; }
    public DateTime? ReceivedDate { get; set; }

    public string? Remark { get; set; }

    public bool IsActive { get; set; } = true;

    public StudentAdmission StudentAdmission { get; set; } = default!;
    public DocumentModel DocumentModel { get; set; } = default!;


}