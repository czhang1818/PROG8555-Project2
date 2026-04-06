using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize]
    public class VolunteersController : Controller
    {
        private readonly IVolunteerService _service;
        private readonly ISkillService _skillService;

        public VolunteersController(IVolunteerService service, ISkillService skillService)
        {
            _service = service;
            _skillService = skillService;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var list = await _service.GetPaginatedVolunteersAsync(pageNumber ?? 1, 10);
            return View(list);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var vol = await _service.GetVolunteerByIdAsync(id.Value);
            if (vol == null) return NotFound();
            return View(vol);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Create(Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                await _service.AddVolunteerAsync(volunteer);
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var vol = await _service.GetVolunteerByIdAsync(id.Value);
            if (vol == null) return NotFound();
            return View(vol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Edit(Guid id, Volunteer volunteer)
        {
            if (id != volunteer.VolunteerId) return NotFound();
            if (ModelState.IsValid)
            {
                await _service.UpdateVolunteerAsync(volunteer);
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var vol = await _service.GetVolunteerByIdAsync(id.Value);
            if (vol == null) return NotFound();
            return View(vol);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteVolunteerAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
