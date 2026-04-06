using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationService _service;
        private readonly IVolunteerService _volService;
        private readonly IOpportunityService _oppService;

        public ApplicationsController(
            IApplicationService service,
            IVolunteerService volService,
            IOpportunityService oppService)
        {
            _service = service;
            _volService = volService;
            _oppService = oppService;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var list = await _service.GetPaginatedApplicationsAsync(pageNumber ?? 1, 10);
            return View(list);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var app = await _service.GetApplicationByIdAsync(id.Value);
            if (app == null) return NotFound();
            return View(app);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Create()
        {
            ViewData["VolunteerId"] = new SelectList(
                await _volService.GetAllVolunteersAsync(), "VolunteerId", "Name");
            ViewData["OpportunityId"] = new SelectList(
                await _oppService.GetAllOpportunitiesAsync(), "OpportunityId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Create(Application application)
        {
            if (ModelState.IsValid)
            {
                // Audit tracking
                application.CreatedByUserId = User.Identity?.Name;
                await _service.AddApplicationAsync(application);
                return RedirectToAction(nameof(Index));
            }
            ViewData["VolunteerId"] = new SelectList(
                await _volService.GetAllVolunteersAsync(), "VolunteerId", "Name", application.VolunteerId);
            ViewData["OpportunityId"] = new SelectList(
                await _oppService.GetAllOpportunitiesAsync(), "OpportunityId", "Title", application.OpportunityId);
            return View(application);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var app = await _service.GetApplicationByIdAsync(id.Value);
            if (app == null) return NotFound();
            ViewData["VolunteerId"] = new SelectList(
                await _volService.GetAllVolunteersAsync(), "VolunteerId", "Name", app.VolunteerId);
            ViewData["OpportunityId"] = new SelectList(
                await _oppService.GetAllOpportunitiesAsync(), "OpportunityId", "Title", app.OpportunityId);
            return View(app);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Edit(Guid id, Application application)
        {
            if (id != application.AppId) return NotFound();
            if (ModelState.IsValid)
            {
                // Audit tracking
                application.LastModifiedBy = User.Identity?.Name;
                application.LastModifiedAt = DateTime.UtcNow;
                await _service.UpdateApplicationAsync(application);
                return RedirectToAction(nameof(Index));
            }
            ViewData["VolunteerId"] = new SelectList(
                await _volService.GetAllVolunteersAsync(), "VolunteerId", "Name", application.VolunteerId);
            ViewData["OpportunityId"] = new SelectList(
                await _oppService.GetAllOpportunitiesAsync(), "OpportunityId", "Title", application.OpportunityId);
            return View(application);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var app = await _service.GetApplicationByIdAsync(id.Value);
            if (app == null) return NotFound();
            return View(app);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteApplicationAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
