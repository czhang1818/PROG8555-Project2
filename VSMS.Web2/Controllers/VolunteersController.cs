using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize]
    public class VolunteersController : Controller
    {
        private readonly IVolunteerService _volunteerService;
        private readonly ISkillService _skillService;

        public VolunteersController(IVolunteerService volunteerService, ISkillService skillService)
        {
            _volunteerService = volunteerService;
            _skillService = skillService;
        }

        // GET: Volunteers
        public async Task<IActionResult> Index(int ? pageNumber)
        {
            var volunteerList = await _volunteerService.GetPaginatedVolunteersAsync(pageNumber ?? 1,10);
            return View(volunteerList);
        }

        // GET: Volunteers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _volunteerService.GetVolunteerByIdAsync(id.Value);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        [Authorize(Roles = "Admin, Coordinator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Volunteers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Create(Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                await _volunteerService.AddVolunteerAsync(volunteer);
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _volunteerService.GetVolunteerByIdAsync(id.Value);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Edit(Guid id, Volunteer volunteer)
        {
            if (id != volunteer.VolunteerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _volunteerService.UpdateVolunteerAsync(volunteer);
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _volunteerService.GetVolunteerByIdAsync(id.Value);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Coordinator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _volunteerService.DeleteVolunteerAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
