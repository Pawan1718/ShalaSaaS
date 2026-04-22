using Shala.Shared.Enums;

namespace Shala.Shared.Constants
{
    public static class AppRoles
    {
        public const string SuperAdmin = nameof(AppRole.SuperAdmin);
        public const string SchoolAdmin = nameof(AppRole.SchoolAdmin);
        public const string BranchAdmin = nameof(AppRole.BranchAdmin);
        public const string Teacher = nameof(AppRole.Teacher);
        public const string Accountant = nameof(AppRole.Accountant);
        public const string Staff = nameof(AppRole.Staff);

        public static readonly string[] All =
        {
            SuperAdmin,
            SchoolAdmin,
            BranchAdmin,
            Teacher,
            Accountant,
            Staff
        };

        public static bool IsValid(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            return All.Contains(role.Trim(), StringComparer.OrdinalIgnoreCase);
        }

        public static string Normalize(string role)
        {
            return All.First(x => x.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}