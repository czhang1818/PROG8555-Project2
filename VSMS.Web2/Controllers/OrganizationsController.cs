using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize]
    public class OrganizationsController : Controller
    {
        private readonly IOrganizationService _service;
        public OrganizationsController(IOrganizationService service) { _service = service; }

        // GET: Organizations
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var list = await _service.GetPaginatedOrganizationsAsync(pageNumber ?? 1, 10);
            return View(list);
        }

        // GET: Organizations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var org = await _service.GetOrganizationByIdAsync(id.Value);
            if (org == null) return NotFound();
            return View(org);
        }

        // GET: Organizations/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // POST: Organizations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                await _service.AddOrganizationAsync(organization);
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        // GET: Organizations/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var org = await _service.GetOrganizationByIdAsync(id.Value);
            if (org == null) return NotFound();
            return View(org);
        }

        // POST: Organizations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, Organization organization)
        {
            if (id != organization.OrganizationId) return NotFound();
            if (ModelState.IsValid)
            {
                await _service.UpdateOrganizationAsync(organization);
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        // GET: Organizations/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var org = await _service.GetOrganizationByIdAsync(id.Value);
            if (org == null) return NotFound();
            return View(org);
        }

        // POST: Organizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteOrganizationAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
