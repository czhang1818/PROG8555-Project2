using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationService _appService;
        private readonly IVolunteerService _volunteerService;
        private readonly IOpportunityService _opportunityService;

        public ApplicationsController(IApplicationService appService, IVolunteerService volunteerService, IOpportunityService opportunityService)
        {
            _appService = appService;
            _volunteerService = volunteerService;
            _opportunityService = opportunityService;
        }

        // GET: Applications
        public async Task<IActionResult> Index(int ? pageNumber)
        {
            var appList = await _appService.GetPaginatedApplicationsAsync(pageNumber ?? 1, 10);
            return View(appList);
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _appService.GetApplicationByIdAsync(id.Value);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications/Create
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Create()
        {
            ViewData["OpportunityId"] = new SelectList(await _opportunityService.GetAllOpportunitiesAsync(), "OpportunityId", "Title");
            ViewData["VolunteerId"] = new SelectList(await _volunteerService.GetAllVolunteersAsync(), "VolunteerId", "Name");
            return View();
        }

        // POST: Applications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Create(Application application)
        {
            if (ModelState.IsValid)
            {
                application.CreatedByUserId = User.Identity?.Name;
                await _appService.AddApplicationAsync(application);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OpportunityId"] = new SelectList(await _opportunityService.GetAllOpportunitiesAsync(), "OpportunityId", "Title", application.OpportunityId);
            ViewData["VolunteerId"] = new SelectList(await _volunteerService.GetAllVolunteersAsync(), "VolunteerId", "Name", application.VolunteerId);
            return View(application);
        }

        // GET: Applications/Edit/5
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _appService.GetApplicationByIdAsync(id.Value);
            if (application == null)
            {
                return NotFound();
            }
            ViewData["OpportunityId"] = new SelectList(await _opportunityService.GetAllOpportunitiesAsync(), "OpportunityId", "Title", application.OpportunityId);
            ViewData["VolunteerId"] = new SelectList(await _volunteerService.GetAllVolunteersAsync(), "VolunteerId", "Name", application.VolunteerId);
            return View(application);
        }

        // POST: Applications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Edit(Guid id, Application application)
        {
            if (id != application.AppId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                application.LastModifiedBy = User.Identity?.Name;
                application.LastModifiedAt = DateTime.UtcNow;
                await _appService.UpdateApplicationAsync(application);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OpportunityId"] = new SelectList(await _opportunityService.GetAllOpportunitiesAsync(), "OpportunityId", "Title", application.OpportunityId);
            ViewData["VolunteerId"] = new SelectList(await _volunteerService.GetAllVolunteersAsync(), "VolunteerId", "Name", application.VolunteerId);
            return View(application);
        }

        // GET: Applications/Delete/5
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _appService.GetApplicationByIdAsync(id.Value);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _appService.DeleteApplicationAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
