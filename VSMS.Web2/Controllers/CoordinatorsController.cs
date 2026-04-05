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
        private readonly ICoordinatorService _coorService;
        private readonly IOrganizationService _orgService;

        public CoordinatorsController(ICoordinatorService coorService, IOrganizationService orgService)
        {
            _coorService = coorService;
            _orgService = orgService;
        }

        // GET: Coordinators
        public async Task<IActionResult> Index(int ? pageNumber)
        {
            var coorList = await _coorService.GetPaginatedCoordinatorsAsync(pageNumber ?? 1, 10);
            return View(coorList);
        }

        // GET: Coordinators/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coordinator = await _coorService.GetCoordinatorByIdAsync(id.Value);
            if (coordinator == null)
            {
                return NotFound();
            }

            return View(coordinator);
        }

        // GET: Coordinators/Create
        public async Task<IActionResult> Create()
        {
            ViewData["OrganizationId"] = new SelectList(await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name");
            return View();
        }

        // POST: Coordinators/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coordinator coordinator)
        {
            if (ModelState.IsValid)
            {
                await _coorService.AddCoordinatorAsync(coordinator);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", coordinator.OrganizationId);
            return View(coordinator);
        }

        // GET: Coordinators/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coordinator = await _coorService.GetCoordinatorByIdAsync(id.Value);
            if (coordinator == null)
            {
                return NotFound();
            }
            ViewData["OrganizationId"] = new SelectList(await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", coordinator.OrganizationId);
            return View(coordinator);
        }

        // POST: Coordinators/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Coordinator coordinator)
        {
            if (id != coordinator.CoordinatorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _coorService.UpdateCoordinatorAsync(coordinator);
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(await _orgService.GetAllOrganizationsAsync(), "OrganizationId", "Name", coordinator.OrganizationId);
            return View(coordinator);
        }

        // GET: Coordinators/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coordinator = await _coorService.GetCoordinatorByIdAsync(id.Value);
            if (coordinator == null)
            {
                return NotFound();
            }

            return View(coordinator);
        }

        // POST: Coordinators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _coorService.DeleteCoordinatorAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
