using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Shared.Requests.StudentDocument
{
    public class UpdateStudentDocumentRequest
    {
        public int Id { get; set; }

        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public bool IsRequired { get; set; }
        public string? Remarks { get; set; }
    }
}
