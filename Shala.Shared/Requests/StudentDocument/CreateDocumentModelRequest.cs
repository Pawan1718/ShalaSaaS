using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Shared.Requests.StudentDocument
{
    public class CreateDocumentModelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }

        public bool IsRequired { get; set; }
        public bool IsAiValidationEnabled { get; set; }
        public bool BlockAdmissionOnMismatch { get; set; }

        public string? AllowedFileTypes { get; set; }
        public int? MaxFileSizeInKb { get; set; }
        public string? RequiredFieldsJson { get; set; }
        public int DisplayOrder { get; set; }
    }
}
