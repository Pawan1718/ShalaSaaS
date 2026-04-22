using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Shared.Requests.StudentDocument
{
    public class ToggleDocumentModelStatusRequest
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
