using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SkillsController : Controller
    {
        private readonly ISkillService _service;
        public SkillsController(ISkillService service) { _service = service; }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var list = await _service.GetPaginatedSkillsAsync(pageNumber ?? 1, 10);
            return View(list);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            var skill = await _service.GetSkillByIdAsync(id.Value);
            if (skill == null) return NotFound();
            return View(skill);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                await _service.AddSkillAsync(skill);
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var skill = await _service.GetSkillByIdAsync(id.Value);
            if (skill == null) return NotFound();
            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Skill skill)
        {
            if (id != skill.SkillId) return NotFound();
            if (ModelState.IsValid)
            {
                await _service.UpdateSkillAsync(skill);
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var skill = await _service.GetSkillByIdAsync(id.Value);
            if (skill == null) return NotFound();
            return View(skill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteSkillAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
