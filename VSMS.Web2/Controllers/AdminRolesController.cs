using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.ViewModels;

namespace VSMS.Web2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRolesController : Controller
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AdminRolesController(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        // GET: AdminRoles
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var viewModels = new List<AdminRoleViewModel>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                viewModels.Add(new AdminRoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name ?? "",
                    UserCount = usersInRole.Count
                });
            }

            return View(viewModels);
        }

        // GET: AdminRoles/Create
        public IActionResult Create() => View();

        // POST: AdminRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "Role name is required.");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        // GET: AdminRoles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var role = await _roleManager.FindByIdAsync(id.Value.ToString());
            if (role == null) return NotFound();
            return View(role);
        }

        // POST: AdminRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, string roleName)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return NotFound();

            role.Name = roleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(role);
        }

        // GET: AdminRoles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var role = await _roleManager.FindByIdAsync(id.Value.ToString());
            if (role == null) return NotFound();

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            ViewData["UserCount"] = usersInRole.Count;
            return View(role);
        }

        // POST: AdminRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminRoles/Assign
        public async Task<IActionResult> Assign(int? pageNumber)
        {
            const int pageSize = 8;
            int currentPage = pageNumber ?? 1;

            var query = GetAssignmentsQuery();
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedAssignments = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["PageIndex"] = currentPage;
            ViewData["TotalPages"] = totalPages;

            var vm = new AssignRoleViewModel
            {
                Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Email"),
                Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name"),
                Assignments = pagedAssignments
            };
            return View(vm);
        }

        // POST: AdminRoles/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.SelectedUserId.ToString());
            if (user != null && !string.IsNullOrEmpty(model.SelectedRoleName))
            {
                if (!await _userManager.IsInRoleAsync(user, model.SelectedRoleName))
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRoleName);
                }
            }

            return RedirectToAction(nameof(Assign));
        }

        // POST: AdminRoles/RemoveAssignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAssignment(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
            return RedirectToAction(nameof(Assign));
        }

        // Returns IQueryable so pagination happens at SQL level
        private IQueryable<UserRoleEntry> GetAssignmentsQuery()
        {
            return from ur in _context.UserRoles
                   join u in _context.Users on ur.UserId equals u.Id
                   join r in _context.Roles on ur.RoleId equals r.Id
                   orderby u.Email, r.Name
                   select new UserRoleEntry
                   {
                       UserId = u.Id,
                       Email = u.Email ?? "",
                       RoleName = r.Name ?? ""
                   };
        }
    }
}
