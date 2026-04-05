using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize]
    public class OpportunitiesController : Controller
    {
        private readonly IOpportunityService _opportunityService;
        private readonly IOrganizationService _orgService;

        public OpportunitiesController(IOpportunityService opportunityService, IOrganizationService organizationService)
        {
            _opportunityService = opportunityService;
            _orgService = organizationService;
        }

        // GET: Opportunities
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var opportunityList = await _opportunityService.GetPaginatedOpportunitiesAsync(pageNumber ?? 1, 10);
            return View(opportunityList);
        }

        // GET: Opportunities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _opportunityService.GetOpportunityByIdAsync(id.Value);
            if (opportunity == null)
            {
                return NotFound();
            }

            return View(opportunity);
        }

        // GET: Opportunities/Create
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Create()
        {
            ViewData["OrganizationId"] = new SelectList(await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name");
            return View();
        }

        // POST: Opportunities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Create(Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                await _opportunityService.AddOpportunityAsync(opportunity);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name");
            return View(opportunity);
        }

        // GET: Opportunities/Edit/5
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _opportunityService.GetOpportunityByIdAsync(id.Value);
            if (opportunity == null)
            {
                return NotFound();
            }
            ViewData["OrganizationId"] = new SelectList(await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name");
            return View(opportunity);
        }

        // POST: Opportunities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Edit(Guid id, Opportunity opportunity)
        {
            if (id != opportunity.OpportunityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _opportunityService.UpdateOpportunityAsync(opportunity);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name");
            return View(opportunity);
        }

        // GET: Opportunities/Delete/5
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _opportunityService.GetOpportunityByIdAsync(id.Value);
            if (opportunity == null)
            {
                return NotFound();
            }

            return View(opportunity);
        }

        // POST: Opportunities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _opportunityService.DeleteOpportunityAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
