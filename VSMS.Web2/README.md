# VSMS — Volunteer Services Management System

An ASP.NET Core MVC application for managing volunteer services, organizations, and opportunities.

---

## 🚀 Quick Start

### Prerequisites
- .NET 10 SDK
- SQLite (included, no setup needed)

### Run the Application
```bash
cd VSMS.Web2
dotnet run
```
Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`.

---

## 🔐 Test Accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@vsms.com` | `Admin@123` |

> New users can register via the Register page and are automatically assigned the **Volunteer** role.

---

## 👥 Role Permissions

| Feature | Admin | Coordinator | Volunteer |
|---|---|---|---|
| Dashboard | ✅ | ✅ | ✅ |
| Organizations (CRUD) | ✅ | ✅ | ✅ |
| Skills (CRUD) | ✅ | ✅ | ✅ |
| Volunteers (CRUD) | ✅ | ✅ | ✅ |
| Applications (CRUD) | ✅ | ✅ | ✅ |
| Opportunities (CRUD) | ✅ | ✅ | ❌ |
| Coordinators (CRUD) | ✅ | ❌ | ❌ |
| Manage Users | ✅ | ❌ | ❌ |
| Manage Roles | ✅ | ❌ | ❌ |

---

## 🏗️ Project Structure

```
VSMS.Web2/
├── Controllers/           # 10 Controllers (Home, Account, Dashboard, 6 Business, 2 Admin)
├── Models/                # 10 Entity Models (ApplicationUser, Organization, Opportunity, etc.)
├── ViewModels/            # 11 ViewModels (Login, Register, Dashboard, Admin, Business)
├── Views/                 # 50+ Razor Views organized by controller
│   ├── Home/              # Landing page
│   ├── Dashboard/         # System dashboard with stats
│   ├── Account/           # Login & Register
│   ├── Organizations/     # CRUD views
│   ├── Skills/            # CRUD views
│   ├── Opportunities/     # CRUD views
│   ├── Volunteers/        # CRUD views
│   ├── Coordinators/      # CRUD views
│   ├── Applications/      # CRUD views
│   ├── AdminUsers/        # User management
│   ├── AdminRoles/        # Role management
│   └── Shared/            # Layout, partials
├── Services/              # 6 Service interfaces + 6 implementations
├── Data/                  # AppDbContext + RoleSeeder
├── Migrations/            # EF Core migrations
├── wwwroot/               # Static files (CSS, JS, libraries)
└── Program.cs             # Application configuration
```

---

## 🔧 Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 10)
- **Authentication**: ASP.NET Core Identity
- **Database**: SQLite with Entity Framework Core
- **UI**: Bootstrap 5 + Custom Glassmorphism CSS
- **Architecture**: MVC + Service Layer + Repository Pattern

---

## 📊 Key Features

1. **Identity Authentication** — Register, Login, Logout with password hashing and lockout policy
2. **Role-Based Authorization** — Admin, Coordinator, Volunteer with menu visibility control
3. **Full CRUD** — 6 business modules with pagination and foreign key dropdowns
4. **Admin Panel** — User management (CRUD, lock/unlock, password reset) + Role management (CRUD, assign/remove)
5. **Audit Tracking** — Application records track who created/modified them
6. **Dashboard** — Real-time stats, application status breakdown, recent opportunities
7. **Anti-Forgery Protection** — `[ValidateAntiForgeryToken]` on all POST actions
8. **Self-Protection** — Admin cannot delete, lock, or demote themselves

---

## 👨‍💻 Team Contributions

| Member | Branch | Scope | Files |
|---|---|---|---|
| M1 | `base` | Foundation: Models, Services, Data, Identity, Layout | ~50 |
| M2 | `feature/member2` | Business CRUD: 6 Controllers + 30 Views | 36 |
| M3 | `feature/member3` | Admin Backend: 2 Controllers + 4 ViewModels + 10 Views | 16 |
| M4 | `feature/member4` | UI/UX: CSS rewrite + Dashboard + README + Document | ~6 |
