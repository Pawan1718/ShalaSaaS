using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Shared.Requests.StudentDocument
{
    public class ToggleStudentDocumentStatusRequest
    {
        public int StudentDocumentId { get; set; }
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
    }
}
