using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.ViewModels;

namespace VSMS.Web2.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                TotalVolunteers = await _context.Volunteers.CountAsync(),
                TotalOrganizations = await _context.Organizations.CountAsync(),
                TotalOpportunities = await _context.Opportunities.CountAsync(),
                TotalApplications = await _context.Applications.CountAsync(),
                TotalSkills = await _context.Skills.CountAsync(),
                TotalCoordinators = await _context.Coordinators.CountAsync(),
                PendingApplications = await _context.Applications.CountAsync(a => a.Status == "Pending"),
                ApprovedApplications = await _context.Applications.CountAsync(a => a.Status == "Approved"),
                RejectedApplications = await _context.Applications.CountAsync(a => a.Status == "Rejected"),
                RecentActivities = await _context.Opportunities
                    .OrderByDescending(o => o.EventDate)
                    .Take(5)
                    .Select(o => new RecentActivityItem
                    {
                        Title = o.Title,
                        Description = o.Organization != null ? o.Organization.Name : "Unknown",
                        Date = o.EventDate,
                        Icon = "🎯"
                    })
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}
