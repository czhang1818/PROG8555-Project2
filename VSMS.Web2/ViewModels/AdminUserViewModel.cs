namespace VSMS.Web2.ViewModels
{
    public class AdminUserViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
