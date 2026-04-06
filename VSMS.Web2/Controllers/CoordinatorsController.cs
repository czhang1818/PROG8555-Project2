using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoordinatorsController : Controller
    {
        private readonly ICoordinatorService _service;
        private readonly IOrganizationService _orgService;

        public CoordinatorsController(ICoordinatorService service, IOrganizationService orgService)
        {
            _service = service;
            _orgService = orgService;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var list = await _service.GetPaginatedCoordinatorsAsync(pageNumber ?? 1, 10);
            return View(list);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var coord = await _service.GetCoordinatorByIdAsync(id.Value);
            if (coord == null) return NotFound();
            return View(coord);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["OrganizationId"] = new SelectList(
                await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coordinator coordinator)
        {
            if (ModelState.IsValid)
            {
                await _service.AddCoordinatorAsync(coordinator);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(
                await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", coordinator.OrganizationId);
            return View(coordinator);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var coord = await _service.GetCoordinatorByIdAsync(id.Value);
            if (coord == null) return NotFound();
            ViewData["OrganizationId"] = new SelectList(
                await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", coord.OrganizationId);
            return View(coord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Coordinator coordinator)
        {
            if (id != coordinator.CoordinatorId) return NotFound();
            if (ModelState.IsValid)
            {
                await _service.UpdateCoordinatorAsync(coordinator);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(
                await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", coordinator.OrganizationId);
            return View(coordinator);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var coord = await _service.GetCoordinatorByIdAsync(id.Value);
            if (coord == null) return NotFound();
            return View(coord);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteCoordinatorAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
