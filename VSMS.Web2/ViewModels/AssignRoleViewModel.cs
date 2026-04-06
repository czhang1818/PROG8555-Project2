using Microsoft.AspNetCore.Mvc.Rendering;

namespace VSMS.Web2.ViewModels
{
    public class AssignRoleViewModel
    {
        public Guid SelectedUserId { get; set; }
        public string SelectedRoleName { get; set; } = string.Empty;

        public SelectList? Users { get; set; }
        public SelectList? Roles { get; set; }

        // Currently assigned user-role pairs for display
        public List<UserRoleEntry> Assignments { get; set; } = new();
    }

    public class UserRoleEntry
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
