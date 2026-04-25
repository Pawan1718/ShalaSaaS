using Shala.Domain.Common;

namespace Shala.Domain.Entities.StudentDocuments;

public class DocumentModel : AuditableEntity, ITenantEntity, IBranchEntity
{
    public int TenantId { get; set; }
    public int BranchId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public bool IsRequired { get; set; } = true;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}