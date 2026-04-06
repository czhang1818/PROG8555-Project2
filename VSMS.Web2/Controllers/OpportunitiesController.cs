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
        private readonly IOpportunityService _service;
        private readonly IOrganizationService _orgService;

        public OpportunitiesController(IOpportunityService service, IOrganizationService orgService)
        {
            _service = service;
            _orgService = orgService;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var list = await _service.GetPaginatedOpportunitiesAsync(pageNumber ?? 1, 10);
            return View(list);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var opp = await _service.GetOpportunityByIdAsync(id.Value);
            if (opp == null) return NotFound();
            return View(opp);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Create()
        {
            ViewData["OrganizationId"] = new SelectList(
                await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Create(Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                await _service.AddOpportunityAsync(opportunity);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(
                await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", opportunity.OrganizationId);
            return View(opportunity);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var opp = await _service.GetOpportunityByIdAsync(id.Value);
            if (opp == null) return NotFound();
            ViewData["OrganizationId"] = new SelectList(
                await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", opp.OrganizationId);
            return View(opp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Edit(Guid id, Opportunity opportunity)
        {
            if (id != opportunity.OpportunityId) return NotFound();
            if (ModelState.IsValid)
            {
                await _service.UpdateOpportunityAsync(opportunity);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(
                await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", opportunity.OrganizationId);
            return View(opportunity);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var opp = await _service.GetOpportunityByIdAsync(id.Value);
            if (opp == null) return NotFound();
            return View(opp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteOpportunityAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
