# PROG8555 - Microsoft Web Technologies
# Group Project - Part 2
# Secure User System, Roles, and Admin Dashboard

## Group Information

| Name | Student Number |
|---|---|
| Bo Yang | XXXXXXXX |
| Bo Zhang | XXXXXXXX |
| Chunxi Zhang | XXXXXXXX |
| Marieth Franciss Perez Zevallos | XXXXXXXX |



## GitHub Repository

https://github.com/czhang1818/PROG8555-Project2

## Test Accounts

| Role | Email | Password |
|---|---|---|
| Admin | admin@vsms.com | Admin@123 |
| Coordinator | sarah.mitchell@redcross.ca | Pass@123 |
| Volunteer | emily.chen@uwaterloo.ca | Pass@123 |

---

## 1. System Overview

VSMS (Volunteer Services Management System) is an ASP.NET Core MVC application that enables organizations to post volunteer opportunities and allows volunteers to discover and apply for them. The system uses ASP.NET Core Identity for authentication and role-based authorization.

**Tech Stack:**
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core 10 with SQLite
- ASP.NET Core Identity with Guid primary keys
- Bootstrap 5 for UI

---

## 2. Screenshots

### 2.1 Home Page (Not Logged In)

![Home Page](screenshots/01_home.png)

### 2.2 Login Page

![Login Page](screenshots/02_login.png)

### 2.3 Register Page

![Register Page](screenshots/03_register.png)

### 2.4 Home Page (Logged In as Admin)

![Home Logged In](screenshots/04_home_logged_in.png)

### 2.5 Dashboard

![Dashboard](screenshots/05_dashboard.png)

### 2.6 Organizations List

![Organizations](screenshots/06_organizations.png)

### 2.7 Opportunities List

![Opportunities](screenshots/07_opportunities.png)

### 2.8 Volunteers List

![Volunteers](screenshots/08_volunteers.png)

### 2.9 Coordinators List

![Coordinators](screenshots/09_coordinators.png)

### 2.10 Applications List

![Applications](screenshots/10_applications.png)

### 2.11 Skills List

![Skills](screenshots/11_skills.png)

### 2.12 Admin - Manage Users

![Manage Users](screenshots/12_admin_users.png)

### 2.13 Admin - Create User

![Create User](screenshots/13_admin_create_user.png)

### 2.14 Admin - Manage Roles

![Manage Roles](screenshots/14_admin_roles.png)

### 2.15 Admin - Assign Roles

![Assign Roles](screenshots/15_admin_assign_role.png)

---

## 3. Program.cs Configuration

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.Services;
using VSMS.Web2.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add MVC with global anti-forgery protection
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Database Context (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    // Lockout policy
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;

    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

// Register application services (DI)
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IOpportunityService, OpportunityService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ICoordinatorService, CoordinatorService>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();

var app = builder.Build();

// Seed roles and default admin
using (var scope = app.Services.CreateScope())
{
    await RoleSeeder.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

Key configurations:
- **Anti-Forgery:** Global `AutoValidateAntiforgeryTokenAttribute` filter applied to all POST actions.
- **Identity:** Configured with lockout policy (3 attempts, 5 min), strong password requirements, and unique email.
- **Cookie Auth:** Custom login/logout paths.
- **DI:** Service layer with interfaces registered as scoped services.
- **Role Seeding:** On startup, seeds Admin, Coordinator, and Volunteer roles plus a default admin account.

---

## 4. Models

### 4.1 ApplicationUser

```csharp
public class ApplicationUser : IdentityUser<Guid>
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### 4.2 Organization

```csharp
public class Organization
{
    [Key]
    public Guid OrganizationId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Contact Email")]
    public string ContactEmail { get; set; } = string.Empty;

    [Url]
    public string? Website { get; set; }

    [Display(Name = "Is Verified")]
    public bool IsVerified { get; set; } = false;

    // Navigation Properties
    public ICollection<Opportunity>? Opportunities { get; set; }
    public ICollection<Coordinator>? Coordinators { get; set; }
}
```

### 4.3 Opportunity

```csharp
public class Opportunity
{
    [Key]
    public Guid OpportunityId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Event Date")]
    public DateTime EventDate { get; set; } = DateTime.Now;

    [Required]
    public string Location { get; set; } = string.Empty;

    [Range(1, 1000)]
    [Display(Name = "Max Volunteers")]
    public int MaxVolunteers { get; set; }

    // Navigation Properties
    [ForeignKey("OrganizationId")]
    public Organization? Organization { get; set; }
    public ICollection<Application>? Applications { get; set; }
    public ICollection<OpportunitySkill>? OpportunitySkills { get; set; }
}
```

### 4.4 Volunteer

```csharp
public class Volunteer
{
    [Key]
    public Guid VolunteerId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Total Hours")]
    public float TotalHours { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK to ApplicationUser (1-to-1)
    public Guid? UserId { get; set; }
    [ForeignKey("UserId")]
    public ApplicationUser? ApplicationUser { get; set; }

    // Navigation Properties
    public ICollection<Application>? Applications { get; set; }
    public ICollection<VolunteerSkill>? VolunteerSkills { get; set; }
}
```

### 4.5 Coordinator

```csharp
public class Coordinator
{
    [Key]
    public Guid CoordinatorId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job Title")]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK to ApplicationUser (1-to-1)
    public Guid? UserId { get; set; }
    [ForeignKey("UserId")]
    public ApplicationUser? ApplicationUser { get; set; }

    // Navigation Property
    [ForeignKey("OrganizationId")]
    public Organization? Organization { get; set; }
}
```

### 4.6 Application (Main Domain Table with Audit Tracking)

```csharp
public class Application
{
    [Key]
    public Guid AppId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid VolunteerId { get; set; }

    [Required]
    public Guid OpportunityId { get; set; }

    [Display(Name = "Submission Date")]
    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = "Pending";

    // Audit tracking fields
    [Display(Name = "Created By")]
    public string? CreatedByUserId { get; set; }

    [Display(Name = "Last Modified By")]
    public string? LastModifiedBy { get; set; }

    [Display(Name = "Last Modified At")]
    public DateTime? LastModifiedAt { get; set; }

    // Navigation Properties
    [ForeignKey("VolunteerId")]
    public Volunteer? Volunteer { get; set; }

    [ForeignKey("OpportunityId")]
    public Opportunity? Opportunity { get; set; }
}
```

### 4.7 Skill

```csharp
public class Skill
{
    [Key]
    public Guid SkillId { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Requires Certification")]
    public bool RequiresCertification { get; set; }

    // Navigation Properties
    public ICollection<VolunteerSkill>? VolunteerSkills { get; set; }
    public ICollection<OpportunitySkill>? OpportunitySkills { get; set; }
}
```

### 4.8 Junction Tables

```csharp
// VolunteerSkill (Composite PK)
public class VolunteerSkill
{
    public Guid VolunteerId { get; set; }
    public Guid SkillId { get; set; }
    public string ProficiencyLevel { get; set; } = string.Empty;
    public DateTime AcquiredDate { get; set; }
    public Volunteer? Volunteer { get; set; }
    public Skill? Skill { get; set; }
}

// OpportunitySkill (Composite PK)
public class OpportunitySkill
{
    public Guid OpportunityId { get; set; }
    public Guid SkillId { get; set; }
    public bool IsMandatory { get; set; }
    public string MinimumLevel { get; set; } = string.Empty;
    public Opportunity? Opportunity { get; set; }
    public Skill? Skill { get; set; }
}
```

---

## 5. AppDbContext

```csharp
public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Volunteer> Volunteers { get; set; }
    public DbSet<Coordinator> Coordinators { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Opportunity> Opportunities { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<VolunteerSkill> VolunteerSkills { get; set; }
    public DbSet<OpportunitySkill> OpportunitySkills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite primary key for VolunteerSkill
        modelBuilder.Entity<VolunteerSkill>()
            .HasKey(vs => new { vs.VolunteerId, vs.SkillId });
        // ... relationships configured via Fluent API

        // Composite primary key for OpportunitySkill
        modelBuilder.Entity<OpportunitySkill>()
            .HasKey(os => new { os.OpportunityId, os.SkillId });
        // ... relationships configured via Fluent API
    }
}
```

---

## 6. ViewModels

### 6.1 LoginViewModel

```csharp
public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
```

### 6.2 RegisterViewModel

```csharp
public class RegisterViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
```

### 6.3 AdminUserEditViewModel

```csharp
public class AdminUserEditViewModel
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public string SelectedRole { get; set; } = string.Empty;
    public SelectList? RoleList { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "New Password (leave blank to keep current)")]
    public string? NewPassword { get; set; }
}
```

### 6.4 AssignRoleViewModel

```csharp
public class AssignRoleViewModel
{
    public Guid SelectedUserId { get; set; }
    public string SelectedRoleName { get; set; } = string.Empty;
    public SelectList? Users { get; set; }
    public SelectList? Roles { get; set; }
    public List<UserRoleEntry> Assignments { get; set; } = new();
}
```

### 6.5 DashboardViewModel

```csharp
public class DashboardViewModel
{
    public int TotalVolunteers { get; set; }
    public int TotalOrganizations { get; set; }
    public int TotalOpportunities { get; set; }
    public int TotalApplications { get; set; }
    public int TotalSkills { get; set; }
    public int TotalCoordinators { get; set; }
    public int PendingApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public List<RecentActivityItem> RecentActivities { get; set; } = new();
}
```

---

## 7. Controllers

### 7.1 AccountController (Authentication)

Handles user registration, login, and logout using ASP.NET Core Identity.

```csharp
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Volunteer");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
                return View(model);
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return View(model);
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
```

### 7.2 AdminUsersController (User Management)

Admin-only controller for managing users. Supports CRUD + lock/unlock + password reset.

```csharp
[Authorize(Roles = "Admin")]
public class AdminUsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    // GET: AdminUsers - lists all users with their roles and lock status
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var viewModels = new List<AdminUserViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            viewModels.Add(new AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FullName = user.FullName,
                Roles = roles.ToList(),
                IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                CreatedAt = user.CreatedAt
            });
        }
        return View(viewModels);
    }

    // POST: AdminUsers/Edit - updates email, name, role (dropdown), and optional password reset
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AdminUserEditViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;
            user.FullName = model.FullName;
            await _userManager.UpdateAsync(user);

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }

            // Reset password if provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            }
            return RedirectToAction(nameof(Index));
        }
        model.RoleList = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", model.SelectedRole);
        return View(model);
    }

    // POST: AdminUsers/Lock
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lock(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: AdminUsers/Unlock
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
        }
        return RedirectToAction(nameof(Index));
    }
}
```

### 7.3 AdminRolesController (Role Management)

Admin-only controller for CRUD on roles and assigning/removing roles from users.

```csharp
[Authorize(Roles = "Admin")]
public class AdminRolesController : Controller
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    // GET: AdminRoles/Assign - shows user dropdown, role dropdown, and Assign button
    public async Task<IActionResult> Assign()
    {
        var vm = new AssignRoleViewModel
        {
            Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "Email"),
            Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name"),
            Assignments = await GetAllAssignments()
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
}
```

### 7.4 ApplicationsController (Audit Tracking)

Manages volunteer applications with audit tracking fields.

```csharp
[Authorize]
public class ApplicationsController : Controller
{
    private readonly IApplicationService _appService;

    // POST: Applications/Create - records which user created the record
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Coordinator")]
    public async Task<IActionResult> Create(Application application)
    {
        if (ModelState.IsValid)
        {
            application.CreatedByUserId = User.Identity?.Name;
            await _appService.AddApplicationAsync(application);
            return RedirectToAction(nameof(Index));
        }
        return View(application);
    }

    // POST: Applications/Edit - records who modified the record and when
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Coordinator")]
    public async Task<IActionResult> Edit(Guid id, Application application)
    {
        if (id != application.AppId) return NotFound();
        if (ModelState.IsValid)
        {
            application.LastModifiedBy = User.Identity?.Name;
            application.LastModifiedAt = DateTime.UtcNow;
            await _appService.UpdateApplicationAsync(application);
            return RedirectToAction(nameof(Index));
        }
        return View(application);
    }
}
```

### 7.5 Other Controllers

| Controller | Auth | Description |
|---|---|---|
| DashboardController | `[Authorize]` | System statistics dashboard |
| OrganizationsController | `[Authorize]`, `[Authorize(Roles = "Admin")]` for CUD | Organization CRUD |
| OpportunitiesController | `[Authorize]`, `[Authorize(Roles = "Admin, Coordinator")]` for CUD | Opportunity CRUD |
| VolunteersController | `[Authorize]`, `[Authorize(Roles = "Admin, Coordinator")]` for CUD | Volunteer CRUD |
| CoordinatorsController | `[Authorize(Roles = "Admin")]` | Coordinator CRUD |
| SkillsController | `[Authorize]`, `[Authorize(Roles = "Admin")]` for CUD | Skill CRUD |

---

## 8. Views

### 8.1 _Layout.cshtml (Role-Based Navigation)

The navigation menu shows different items based on the user's role:
- **Not logged in:** Home, Register, Login
- **Logged in (any role):** Home, Dashboard, Organizations, Opportunities, Volunteers, Coordinators, Applications, Skills
- **Admin only:** Admin dropdown with "Manage Users" and "Manage Roles"

```html
@if (User.Identity != null && User.Identity.IsAuthenticated)
{
    <li class="nav-item">
        <a class="nav-link" asp-controller="Dashboard" asp-action="Index">Dashboard</a>
    </li>
    <li class="nav-item">
        <a class="nav-link" asp-controller="Organizations" asp-action="Index">Organizations</a>
    </li>
    <!-- ... other menu items ... -->
}
@if (User.IsInRole("Admin"))
{
    <li class="nav-item dropdown">
        <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
            Admin
        </a>
        <ul class="dropdown-menu">
            <li><a class="dropdown-item" asp-controller="AdminUsers" asp-action="Index">Manage Users</a></li>
            <li><a class="dropdown-item" asp-controller="AdminRoles" asp-action="Index">Manage Roles</a></li>
        </ul>
    </li>
}
```

### 8.2 _LoginPartial.cshtml

Shows the user's name and Logout button when signed in, or Register/Login links otherwise.

```html
@using Microsoft.AspNetCore.Identity
@using VSMS.Web2.Models
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager

<ul class="navbar-nav">
@if (SignInManager.IsSignedIn(User))
{
    var user = await UserManager.GetUserAsync(User);
    <li class="nav-item">
        <span class="nav-link text-muted">Hello, @user?.FullName</span>
    </li>
    <li class="nav-item">
        <form asp-controller="Account" asp-action="Logout" method="post">
            <button type="submit" class="nav-link btn btn-link text-muted">Logout</button>
        </form>
    </li>
}
else
{
    <li class="nav-item">
        <a class="nav-link text-muted" asp-controller="Account" asp-action="Register">Register</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-muted" asp-controller="Account" asp-action="Login">Login</a>
    </li>
}
</ul>
```

### 8.3 Login.cshtml

```html
@model LoginViewModel
@{
    ViewData["Title"] = "Log in";
}

<div class="row justify-content-center">
    <div class="col-md-6">
        <h1>@ViewData["Title"]</h1>
        <form asp-action="Login" asp-route-returnUrl="@ViewData["ReturnUrl"]" method="post">
            <h2>Sign in to your account</h2>
            <hr />
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-floating mb-3">
                <input asp-for="Email" class="form-control" autocomplete="username" />
                <label asp-for="Email"></label>
                <span asp-validation-for="Email" class="text-danger"></span>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Password" class="form-control" autocomplete="current-password" />
                <label asp-for="Password"></label>
                <span asp-validation-for="Password" class="text-danger"></span>
            </div>
            <div class="checkbox mb-3">
                <label asp-for="RememberMe">
                    <input asp-for="RememberMe" class="form-check-input" />
                    @Html.DisplayNameFor(m => m.RememberMe)
                </label>
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Log in</button>
        </form>
        <div class="mt-3">
            <p><a asp-action="Register">Register as a new user</a></p>
        </div>
    </div>
</div>
```

### 8.4 Register.cshtml

```html
@model RegisterViewModel
@{
    ViewData["Title"] = "Register";
}

<div class="row justify-content-center">
    <div class="col-md-6">
        <h1>@ViewData["Title"]</h1>
        <form asp-action="Register" asp-route-returnUrl="@ViewData["ReturnUrl"]" method="post">
            <h2>Create a new account</h2>
            <hr />
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-floating mb-3">
                <input asp-for="FullName" class="form-control" />
                <label asp-for="FullName"></label>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Email" class="form-control" />
                <label asp-for="Email"></label>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="Password" class="form-control" />
                <label asp-for="Password"></label>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="ConfirmPassword" class="form-control" />
                <label asp-for="ConfirmPassword"></label>
            </div>
            <button type="submit" class="w-100 btn btn-lg btn-primary">Register</button>
        </form>
    </div>
</div>
```

### 8.5 AdminUsers/Index.cshtml (User Management List)

```html
@model List<AdminUserViewModel>
@{
    ViewData["Title"] = "Manage Users";
}

<h1>Manage Users</h1>
<p><a asp-action="Create" class="btn btn-primary">Create New User</a></p>

<table class="table table-striped">
    <thead>
        <tr>
            <th>Email</th>
            <th>Full Name</th>
            <th>Roles</th>
            <th>Locked</th>
            <th>Created</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var user in Model)
        {
            <tr>
                <td>@user.Email</td>
                <td>@user.FullName</td>
                <td>@string.Join(", ", user.Roles)</td>
                <td>
                    @if (user.IsLocked)
                    { <span class="badge bg-danger">Locked</span> }
                    else
                    { <span class="badge bg-success">Active</span> }
                </td>
                <td>@user.CreatedAt.ToString("yyyy-MM-dd")</td>
                <td>
                    <a asp-action="Edit" asp-route-id="@user.Id" class="btn btn-sm btn-outline-primary">Edit</a>
                    <a asp-action="Details" asp-route-id="@user.Id" class="btn btn-sm btn-outline-info">Details</a>
                    <!-- Lock/Unlock + Delete buttons -->
                </td>
            </tr>
        }
    </tbody>
</table>
```

### 8.6 AdminUsers/Edit.cshtml (Edit User with Role Dropdown + Password Reset)

```html
@model AdminUserEditViewModel

<h1>Edit User</h1>
<div class="row justify-content-center">
    <div class="col-md-6">
        <form asp-action="Edit">
            <input type="hidden" asp-for="Id" />
            <div class="form-floating mb-3">
                <input asp-for="Email" class="form-control" />
                <label asp-for="Email"></label>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="FullName" class="form-control" />
                <label asp-for="FullName"></label>
            </div>
            <div class="form-floating mb-3">
                <select asp-for="SelectedRole" asp-items="Model.RoleList" class="form-select">
                    <option value="">-- No Role --</option>
                </select>
                <label asp-for="SelectedRole">Role</label>
            </div>
            <div class="form-floating mb-3">
                <input asp-for="NewPassword" class="form-control" />
                <label asp-for="NewPassword"></label>
            </div>
            <button type="submit" class="btn btn-primary">Save</button>
            <a asp-action="Index" class="btn btn-secondary">Back to List</a>
        </form>
    </div>
</div>
```

### 8.7 AdminRoles/Assign.cshtml (Role Assignment with Dropdowns)

```html
@model AssignRoleViewModel

<h1>Assign Roles</h1>
<div class="row justify-content-center">
    <div class="col-md-8">
        <h4>Assign a Role to a User</h4>
        <form asp-action="Assign" method="post">
            <div class="row g-3 align-items-end mb-4">
                <div class="col-md-5">
                    <label class="form-label">User</label>
                    <select asp-for="SelectedUserId" asp-items="Model.Users" class="form-select">
                        <option value="">-- Select User --</option>
                    </select>
                </div>
                <div class="col-md-4">
                    <label class="form-label">Role</label>
                    <select asp-for="SelectedRoleName" asp-items="Model.Roles" class="form-select">
                        <option value="">-- Select Role --</option>
                    </select>
                </div>
                <div class="col-md-3">
                    <button type="submit" class="btn btn-primary w-100">Assign</button>
                </div>
            </div>
        </form>

        <h4>Current Assignments</h4>
        <table class="table table-striped">
            <thead>
                <tr><th>User Email</th><th>Role</th><th>Action</th></tr>
            </thead>
            <tbody>
                @foreach (var entry in Model.Assignments)
                {
                    <tr>
                        <td>@entry.Email</td>
                        <td>@entry.RoleName</td>
                        <td>
                            <form asp-action="RemoveAssignment" method="post" style="display:inline">
                                <input type="hidden" name="userId" value="@entry.UserId" />
                                <input type="hidden" name="roleName" value="@entry.RoleName" />
                                <button type="submit" class="btn btn-sm btn-outline-danger">Remove</button>
                            </form>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>
```

---

## 9. Role Seeder

```csharp
public static class RoleSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "Coordinator", "Volunteer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
            }
        }

        // Create default admin account
        var adminEmail = "admin@vsms.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
```

