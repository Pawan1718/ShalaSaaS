
using Shala.Domain.Entities.Identity;
using Shala.Domain.Entities.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shala.Domain.Entities.Organization
{
    public class UserBranchAccess
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
