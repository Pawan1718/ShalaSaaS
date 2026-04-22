using Shala.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Domain.Entities.StudentDocuments
{
    public class DocumentModel : AuditableEntity, ITenantEntity, IBranchEntity
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }

        public string Name { get; set; } = string.Empty;
        // Aadhaar, Birth Certificate, Transfer Certificate, Marksheet

        public string Code { get; set; } = string.Empty;
        // AADHAAR, BIRTH_CERTIFICATE, TC

        public string? Description { get; set; }

        public bool IsRequired { get; set; } = false;
        public bool IsAiValidationEnabled { get; set; } = true;
        public bool BlockAdmissionOnMismatch { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public string? AllowedFileTypes { get; set; }
        // pdf,jpg,jpeg,png

        public int? MaxFileSizeInKb { get; set; }

        public string? RequiredFieldsJson { get; set; }
        // ["StudentName","FatherName","Dob"]

        public int DisplayOrder { get; set; } = 0;
    }
}
