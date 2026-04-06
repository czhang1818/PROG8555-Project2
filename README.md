# VSMS - Volunteer Scheduling & Management System

A full-stack ASP.NET Core MVC web application for managing volunteer organizations, opportunities, and applications. Built with .NET 10, Entity Framework Core, ASP.NET Identity, and SQLite.

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Setup & Run](#setup--run)
- [User Roles](#user-roles)
- [Default Test Accounts](#default-test-accounts)
- [Running Tests](#running-tests)

---

## Overview

VSMS enables organizations to post volunteer opportunities and allows volunteers to discover and apply for them. Coordinators manage opportunities on behalf of their organizations, while administrators oversee the entire platform, including user and role management.

## Key Features

| Area | Capabilities |
|---|---|
| **Authentication** | Register, Login, Logout with ASP.NET Identity (cookie-based) |
| **Authorization** | Role-based access control (Admin / Coordinator / Volunteer) |
| **Organizations** | CRUD operations for volunteer organizations |
| **Opportunities** | Create and manage volunteer events with date, location, and capacity |
| **Applications** | Volunteers apply to opportunities; status tracking (Pending -> Approved / Rejected) |
| **Skills** | Skill catalogue with many-to-many links to volunteers and opportunities |
| **Coordinators** | Manage coordinators linked to organizations and user accounts |
| **Dashboard** | Summary statistics and recent activity feed |
| **Admin Panel** | User management and role assignment |
| **Security** | Anti-forgery tokens, account lockout, strong password policy, audit fields |

## Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 10)
- **Database:** SQLite via Entity Framework Core 10
- **Identity:** ASP.NET Core Identity with `Guid` primary keys
- **Testing:** xUnit (VSMS.Web2.Test project)
- **Architecture:** Service layer with DI, ViewModels, paginated lists

## Project Structure

```
VSMS.Web2.slnx                 # Solution file
│
├── VSMS.Web2/                  # Main web application
│   ├── Controllers/            # MVC controllers
│   │   ├── AccountController       # Register / Login / Logout
│   │   ├── AdminRolesController    # Role CRUD (Admin only)
│   │   ├── AdminUsersController    # User management (Admin only)
│   │   ├── ApplicationsController  # Volunteer applications
│   │   ├── CoordinatorsController  # Coordinator CRUD
│   │   ├── DashboardController     # Dashboard stats
│   │   ├── HomeController          # Landing page
│   │   ├── OpportunitiesController # Opportunity CRUD
│   │   ├── OrganizationsController # Organization CRUD
│   │   ├── SkillsController        # Skill CRUD
│   │   └── VolunteersController    # Volunteer CRUD
│   ├── Data/
│   │   ├── AppDbContext.cs         # EF Core DbContext
│   │   └── RoleSeeder.cs          # Seeds roles & default admin
│   ├── Models/                 # Domain entities
│   ├── ViewModels/             # Presentation models
│   ├── Views/                  # Razor views
│   ├── Services/               # Business logic layer
│   │   └── Interfaces/         # Service contracts
│   ├── Migrations/             # EF Core migrations
│   ├── wwwroot/                # Static assets (CSS, JS, images)
│   ├── Program.cs              # App entry point & DI configuration
│   └── appsettings.json        # Configuration (connection string)
│
└── VSMS.Web2.Test/             # Unit / integration tests
    ├── DataSeeder/             # Test data seeding utilities
    ├── Fixtures/               # Test fixtures & helpers
    └── Services/               # Service-layer tests
```

## Prerequisites

| Tool | Version |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | **10.0** or later |
| Git | Any recent version |

> **Note:** No external database server is required. The app uses an embedded SQLite database (`Data/vsms.db`).

## Setup & Run

```bash
# 1. Clone the repository
git clone https://github.com/czhang1818/PROG8555-Project2.git
cd PROG8555-Project2

# 2. Restore NuGet packages
dotnet restore

# 3. Apply database migrations (creates/updates vsms.db)
dotnet ef database update --project VSMS.Web2

# 4. Run the application
dotnet run --project VSMS.Web2
```

The app will start on:

- **HTTP:** `http://localhost:5000`
- **HTTPS:** `https://localhost:5001`

Ports are configured in `appsettings.json` under the `Kestrel` section.
On first run, the `RoleSeeder` automatically creates the three roles and the default admin account.

## User Roles

| Role | Access Level |
|---|---|
| **Admin** | Full platform access, manage users, assign roles, CRUD all entities |
| **Coordinator** | Manage opportunities and review volunteer applications for their organization |
| **Volunteer** | Browse opportunities, submit applications, manage own profile and skills |

New users who register through the UI are automatically assigned the **Volunteer** role. Admins can promote users to **Coordinator** or **Admin** via the Admin Panel.

## Default Test Accounts

The following accounts are **automatically seeded** on first run:

| Role | Email | Password |
|---|---|---|
| **Admin** | `admin@vsms.com` | `Admin@123` |
| **Coordinator** | `sarah.mitchell@redcross.ca` | `Pass@123` |
| **Volunteer** | `emily.chen@uwaterloo.ca` | `Pass@123` |

All seeded accounts (except Admin) use the password `Pass@123`.

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal
```

