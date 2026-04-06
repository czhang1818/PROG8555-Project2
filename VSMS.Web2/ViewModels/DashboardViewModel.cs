namespace VSMS.Web2.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalVolunteers { get; set; }
        public int TotalOrganizations { get; set; }
        public int TotalOpportunities { get; set; }
        public int TotalApplications { get; set; }
        public int TotalSkills { get; set; }
        public int TotalCoordinators { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public List<RecentActivityItem> RecentActivities { get; set; } = new();
    }

    public class RecentActivityItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Icon { get; set; } = "📋";
    }
}
