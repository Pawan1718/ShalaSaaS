using Shala.Domain.Entities.StudentDocuments;
using Shala.Shared.Responses.StudentDocument;

namespace Shala.Application.Features.StudentDocument;

public static class StudentDocumentChecklistMappings
{
    public static StudentDocumentChecklistItemResponse ToChecklistItemResponse(
        this DocumentModel model,
        StudentDocumentChecklist? checklist)
    {
        return new StudentDocumentChecklistItemResponse
        {
            DocumentModelId = model.Id,
            DocumentName = model.Name,
            DocumentCode = model.Code,
            Description = model.Description,
            IsRequired = model.IsRequired,
            IsReceived = checklist?.IsReceived ?? false,
            Remark = checklist?.Remark,
            ReceivedDate = checklist?.ReceivedDate,
            DisplayOrder = model.DisplayOrder
        };
    }
}