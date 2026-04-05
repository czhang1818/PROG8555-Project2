using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Controllers
{
    [Authorize]
    public class SkillsController : Controller
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        // GET: Skills
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var skillList = await _skillService.GetPaginatedSkillsAsync(pageNumber ?? 1, 10);
            return View(skillList);
        }

        // GET: Skills/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var skill = await _skillService.GetSkillByIdAsync(id.Value);
            if (skill == null)
            {
                return NotFound();
            }
            return View(skill);
        }

        // GET: Skills/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Skills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                await _skillService.AddSkillAsync(skill);
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Skills/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = await _skillService.GetSkillByIdAsync(id.Value);
            if (skill == null)
            {
                return NotFound();
            }
            return View(skill);
        }

        // POST: Skills/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, Skill skill)
        {
            if (id != skill.SkillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _skillService.UpdateSkillAsync(skill);
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Skills/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = await _skillService.GetSkillByIdAsync(id.Value);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _skillService.DeleteSkillAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
