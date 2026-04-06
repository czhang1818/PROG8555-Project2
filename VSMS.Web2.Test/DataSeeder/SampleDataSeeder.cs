using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;

namespace VSMS.Web2.Test.DataSeeder
{
    /// <summary>
    /// Populates the real VSMS database with production-realistic sample data.
    /// Run via: dotnet test --filter "FullyQualifiedName~DataSeeder"
    /// </summary>
    public class SampleDataSeeder
    {
        [Fact]
        public async Task SeedSampleData()
        {
            var dbPath = FindDatabasePath();
            Assert.True(File.Exists(dbPath), $"Database not found at: {dbPath}");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            using var context = new AppDbContext(options);

            // ── Clean existing non-identity data ──────────────────────────────
            // Preserve AspNet* identity tables, only clear business data
            if (await context.Applications.AnyAsync())
            {
                context.Applications.RemoveRange(await context.Applications.ToListAsync());
                await context.SaveChangesAsync();
            }
            if (await context.VolunteerSkills.AnyAsync())
            {
                context.VolunteerSkills.RemoveRange(await context.VolunteerSkills.ToListAsync());
                await context.SaveChangesAsync();
            }
            if (await context.OpportunitySkills.AnyAsync())
            {
                context.OpportunitySkills.RemoveRange(await context.OpportunitySkills.ToListAsync());
                await context.SaveChangesAsync();
            }
            if (await context.Coordinators.AnyAsync())
            {
                context.Coordinators.RemoveRange(await context.Coordinators.ToListAsync());
                await context.SaveChangesAsync();
            }
            if (await context.Opportunities.AnyAsync())
            {
                context.Opportunities.RemoveRange(await context.Opportunities.ToListAsync());
                await context.SaveChangesAsync();
            }
            if (await context.Volunteers.AnyAsync())
            {
                context.Volunteers.RemoveRange(await context.Volunteers.ToListAsync());
                await context.SaveChangesAsync();
            }
            if (await context.Skills.AnyAsync())
            {
                context.Skills.RemoveRange(await context.Skills.ToListAsync());
                await context.SaveChangesAsync();
            }
            if (await context.Organizations.AnyAsync())
            {
                context.Organizations.RemoveRange(await context.Organizations.ToListAsync());
                await context.SaveChangesAsync();
            }

            // Also clean up seeded user accounts (preserve admin@vsms.com)
            var seededUsers = await context.Users
                .Where(u => u.Email != "admin@vsms.com")
                .ToListAsync();
            if (seededUsers.Any())
            {
                // Remove role assignments for seeded users
                var seededUserIds = seededUsers.Select(u => u.Id).ToList();
                var userRoles = await context.Set<IdentityUserRole<Guid>>()
                    .Where(ur => seededUserIds.Contains(ur.UserId))
                    .ToListAsync();
                context.Set<IdentityUserRole<Guid>>().RemoveRange(userRoles);
                context.Users.RemoveRange(seededUsers);
                await context.SaveChangesAsync();
            }

            Console.WriteLine("🧹 Existing business data cleared.");
            Console.WriteLine("🌱 Seeding production-realistic data...\n");

            // ── 0. Identity Users & Roles ─────────────────────────────────────
            // Create login accounts for volunteers and coordinators
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var now0 = DateTime.UtcNow;

            // Look up existing role IDs
            var volunteerRole = await context.Roles.FirstAsync(r => r.Name == "Volunteer");
            var coordinatorRole = await context.Roles.FirstAsync(r => r.Name == "Coordinator");

            // Helper to create a user with hashed password
            ApplicationUser CreateUser(string email, string fullName, DateTime createdAt)
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = email,
                    NormalizedUserName = email.ToUpperInvariant(),
                    Email = email,
                    NormalizedEmail = email.ToUpperInvariant(),
                    EmailConfirmed = true,
                    FullName = fullName,
                    CreatedAt = createdAt,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = true
                };
                user.PasswordHash = passwordHasher.HashPassword(user, "Pass@123");
                return user;
            }

            // -- Volunteer user accounts (not all volunteers have accounts – realistic) --
            var userEmily = CreateUser("emily.chen@uwaterloo.ca", "Emily Chen", now0.AddDays(-142));
            var userPriya = CreateUser("priya.sharma@conestogac.on.ca", "Priya Sharma", now0.AddDays(-131));
            var userFatima = CreateUser("falrashid@gmail.com", "Fatima Al-Rashid", now0.AddDays(-158));
            var userNoah = CreateUser("noah.b@wlu.ca", "Noah Beaulieu", now0.AddDays(-103));
            var userSophie = CreateUser("sophiet@bell.net", "Sophie Tremblay", now0.AddDays(-145));
            var userJames = CreateUser("j.okafor@conestogac.on.ca", "James Okafor", now0.AddDays(-95));
            var userHannah = CreateUser("hwolfe@uwaterloo.ca", "Hannah Wolfe", now0.AddDays(-76));
            var userRaj = CreateUser("raj.patel@outlook.com", "Raj Patel", now0.AddDays(-120));

            // -- Coordinator user accounts --
            var userSarah = CreateUser("sarah.mitchell@redcross.ca", "Sarah Mitchell", now0.AddDays(-160));
            var userCarlos = CreateUser("carlos.medina@habitatwr.ca", "Carlos Medina", now0.AddDays(-155));
            var userAngela = CreateUser("afung@thefoodbank.ca", "Angela Fung", now0.AddDays(-152));
            var userMichael = CreateUser("mosei@grhosp.on.ca", "Dr. Michael Osei", now0.AddDays(-148));
            var userNadia = CreateUser("npetrov@kwmc.on.ca", "Nadia Petrov", now0.AddDays(-140));
            var userLaura = CreateUser("lbennett@kwhumane.com", "Laura Bennett", now0.AddDays(-90));
            var userThomas = CreateUser("tpark@conestogagreenway.ca", "Thomas Park", now0.AddDays(-80));

            var allVolunteerUsers = new[] { userEmily, userPriya, userFatima, userNoah, userSophie, userJames, userHannah, userRaj };
            var allCoordinatorUsers = new[] { userSarah, userCarlos, userAngela, userMichael, userNadia, userLaura, userThomas };

            context.Users.AddRange(allVolunteerUsers);
            context.Users.AddRange(allCoordinatorUsers);
            await context.SaveChangesAsync();

            // Assign roles
            foreach (var vu in allVolunteerUsers)
                context.Set<IdentityUserRole<Guid>>().Add(new IdentityUserRole<Guid> { UserId = vu.Id, RoleId = volunteerRole.Id });
            foreach (var cu in allCoordinatorUsers)
                context.Set<IdentityUserRole<Guid>>().Add(new IdentityUserRole<Guid> { UserId = cu.Id, RoleId = coordinatorRole.Id });
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✅ {allVolunteerUsers.Length + allCoordinatorUsers.Length} User accounts (password: Pass@123)");
            Console.WriteLine($"      ├─ {allVolunteerUsers.Length} Volunteer accounts");
            Console.WriteLine($"      └─ {allCoordinatorUsers.Length} Coordinator accounts");

            // ── 1. Organizations ──────────────────────────────────────────────
            // A mix of well-known and smaller local orgs, some verified, some pending
            var organizations = new List<Organization>
            {
                new() { Name = "Canadian Red Cross – Waterloo Region", ContactEmail = "waterloo@redcross.ca", Website = "https://www.redcross.ca", IsVerified = true },
                new() { Name = "Habitat for Humanity Waterloo Region", ContactEmail = "volunteer@habitatwr.ca", Website = "https://habitatwr.ca", IsVerified = true },
                new() { Name = "The Food Bank of Waterloo Region", ContactEmail = "info@thefoodbank.ca", Website = "https://www.thefoodbank.ca", IsVerified = true },
                new() { Name = "Grand River Hospital Foundation", ContactEmail = "foundation@grhosp.on.ca", Website = "https://www.grhf.org", IsVerified = true },
                new() { Name = "KW Multicultural Centre", ContactEmail = "programs@kwmc.on.ca", Website = "https://www.kwmulticultural.ca", IsVerified = true },
                new() { Name = "Kitchener-Waterloo Humane Society", ContactEmail = "volunteer@kwhumane.com", Website = "https://kwhumane.com", IsVerified = false },
                new() { Name = "Conestoga Greenway Conservancy", ContactEmail = "hello@conestogagreenway.ca", Website = "https://conestogagreenway.ca", IsVerified = false },
            };
            context.Organizations.AddRange(organizations);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {organizations.Count} Organizations");

            // ── 2. Skills ─────────────────────────────────────────────────────
            var skills = new List<Skill>
            {
                new() { Name = "First Aid / CPR-C", Category = "Safety", Description = "Standard First Aid with CPR Level C & AED", RequiresCertification = true },
                new() { Name = "Food Handler Certification", Category = "Safety", Description = "Ontario Food Handler certificate for safe food prep", RequiresCertification = true },
                new() { Name = "Class G Driver's License", Category = "Logistics", Description = "Valid Ontario G license for driving assignments", RequiresCertification = true },
                new() { Name = "Project Coordination", Category = "Leadership", Description = "Ability to plan timelines, delegate tasks, and manage scope", RequiresCertification = false },
                new() { Name = "Bilingual (English / French)", Category = "Language", Description = "Working proficiency in both official languages", RequiresCertification = false },
                new() { Name = "Carpentry & Basic Construction", Category = "Trades", Description = "Framing, drywall, general residential construction", RequiresCertification = false },
                new() { Name = "Social Media & Marketing", Category = "Communications", Description = "Content creation, Instagram/Facebook management, Canva", RequiresCertification = false },
                new() { Name = "Tutoring / Mentoring", Category = "Education", Description = "One-on-one or small-group academic support", RequiresCertification = false },
                new() { Name = "IT Support", Category = "Technology", Description = "Basic troubleshooting, networking, and helpdesk", RequiresCertification = false },
                new() { Name = "Animal Handling", Category = "Specialized", Description = "Safe interaction with shelter and rescue animals", RequiresCertification = false },
            };
            context.Skills.AddRange(skills);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {skills.Count} Skills");

            // ── 3. Volunteers ─────────────────────────────────────────────────
            // Staggered registration dates over the past ~5 months
            var now = DateTime.UtcNow;
            var volunteers = new List<Volunteer>
            {
                new() { Name = "Emily Chen", Email = "emily.chen@uwaterloo.ca", PhoneNumber = "519-722-4831", TotalHours = 67.5f, CreatedAt = now.AddDays(-142), UserId = userEmily.Id },
                new() { Name = "Marcus Johnson", Email = "marcusj94@gmail.com", PhoneNumber = "226-868-1293", TotalHours = 23.0f, CreatedAt = now.AddDays(-118) },  // No login account
                new() { Name = "Priya Sharma", Email = "priya.sharma@conestogac.on.ca", PhoneNumber = "548-333-7712", TotalHours = 104.5f, CreatedAt = now.AddDays(-131), UserId = userPriya.Id },
                new() { Name = "Liam MacKenzie", Email = "liammack@outlook.com", PhoneNumber = "519-590-2048", TotalHours = 8.5f, CreatedAt = now.AddDays(-87) },  // No login account
                new() { Name = "Fatima Al-Rashid", Email = "falrashid@gmail.com", PhoneNumber = "226-505-9167", TotalHours = 156.0f, CreatedAt = now.AddDays(-158), UserId = userFatima.Id },
                new() { Name = "Noah Beaulieu", Email = "noah.b@wlu.ca", PhoneNumber = "519-404-3321", TotalHours = 41.0f, CreatedAt = now.AddDays(-103), UserId = userNoah.Id },
                new() { Name = "Isabella Rivera", Email = "isabellar@hotmail.com", PhoneNumber = "548-219-6640", TotalHours = 12.0f, CreatedAt = now.AddDays(-62) },  // No login account
                new() { Name = "Owen Taylor", Email = "owen.t.2001@gmail.com", PhoneNumber = "226-777-1482", TotalHours = 0f, CreatedAt = now.AddDays(-14) },  // No login account – just registered
                new() { Name = "Sophie Tremblay", Email = "sophiet@bell.net", PhoneNumber = "519-635-5590", TotalHours = 89.0f, CreatedAt = now.AddDays(-145), UserId = userSophie.Id },
                new() { Name = "James Okafor", Email = "j.okafor@conestogac.on.ca", PhoneNumber = "548-881-2037", TotalHours = 34.5f, CreatedAt = now.AddDays(-95), UserId = userJames.Id },
                new() { Name = "Hannah Wolfe", Email = "hwolfe@uwaterloo.ca", PhoneNumber = "519-496-3318", TotalHours = 19.5f, CreatedAt = now.AddDays(-76), UserId = userHannah.Id },
                new() { Name = "Raj Patel", Email = "raj.patel@outlook.com", PhoneNumber = "226-332-8845", TotalHours = 52.0f, CreatedAt = now.AddDays(-120), UserId = userRaj.Id },
            };
            context.Volunteers.AddRange(volunteers);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {volunteers.Count} Volunteers (8 linked to user accounts, 4 without login)");

            // ── 4. Coordinators ───────────────────────────────────────────────
            var coordinators = new List<Coordinator>
            {
                new() { OrganizationId = organizations[0].OrganizationId, Name = "Sarah Mitchell", JobTitle = "Disaster Response Coordinator", Email = "sarah.mitchell@redcross.ca", PhoneNumber = "519-745-8200 x201", CreatedAt = now.AddDays(-160), UserId = userSarah.Id },
                new() { OrganizationId = organizations[1].OrganizationId, Name = "Carlos Medina", JobTitle = "Build Site Supervisor", Email = "carlos.medina@habitatwr.ca", PhoneNumber = "519-747-0664 x105", CreatedAt = now.AddDays(-155), UserId = userCarlos.Id },
                new() { OrganizationId = organizations[2].OrganizationId, Name = "Angela Fung", JobTitle = "Warehouse Operations Lead", Email = "afung@thefoodbank.ca", PhoneNumber = "519-743-5576 x302", CreatedAt = now.AddDays(-152), UserId = userAngela.Id },
                new() { OrganizationId = organizations[3].OrganizationId, Name = "Dr. Michael Osei", JobTitle = "Community Engagement Manager", Email = "mosei@grhosp.on.ca", PhoneNumber = "519-749-4300 x412", CreatedAt = now.AddDays(-148), UserId = userMichael.Id },
                new() { OrganizationId = organizations[4].OrganizationId, Name = "Nadia Petrov", JobTitle = "Settlement Program Coordinator", Email = "npetrov@kwmc.on.ca", PhoneNumber = "519-745-2531 x118", CreatedAt = now.AddDays(-140), UserId = userNadia.Id },
                new() { OrganizationId = organizations[5].OrganizationId, Name = "Laura Bennett", JobTitle = "Volunteer Services Coordinator", Email = "lbennett@kwhumane.com", PhoneNumber = "519-745-5615 x22", CreatedAt = now.AddDays(-90), UserId = userLaura.Id },
                new() { OrganizationId = organizations[6].OrganizationId, Name = "Thomas Park", JobTitle = "Trail Stewardship Lead", Email = "tpark@conestogagreenway.ca", PhoneNumber = "226-243-0091", CreatedAt = now.AddDays(-80), UserId = userThomas.Id },
            };
            context.Coordinators.AddRange(coordinators);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {coordinators.Count} Coordinators (all linked to user accounts)");

            // ── 5. Opportunities ──────────────────────────────────────────────
            // Mix of PAST completed events, CURRENT upcoming, and FUTURE planned
            var opportunities = new List<Opportunity>
            {
                // ---- Past (completed) ----
                new() { OrganizationId = organizations[2].OrganizationId, Title = "Thanksgiving Food Drive Sorting", EventDate = new DateTime(2025, 10, 11, 9, 0, 0), Location = "50 Alpine Ct, Kitchener", MaxVolunteers = 35 },
                new() { OrganizationId = organizations[0].OrganizationId, Title = "Winter Storm Emergency Shelter", EventDate = new DateTime(2026, 1, 18, 18, 0, 0), Location = "Kitchener Memorial Auditorium, 400 East Ave", MaxVolunteers = 20 },
                new() { OrganizationId = organizations[1].OrganizationId, Title = "Home Build #47 – Framing Weekend", EventDate = new DateTime(2026, 2, 8, 7, 30, 0), Location = "162 Courtland Ave E, Kitchener", MaxVolunteers = 24 },
                new() { OrganizationId = organizations[4].OrganizationId, Title = "Newcomer Welcome Session – February", EventDate = new DateTime(2026, 2, 22, 13, 0, 0), Location = "102 King St W, Kitchener", MaxVolunteers = 12 },
                new() { OrganizationId = organizations[3].OrganizationId, Title = "Hospital Gift Shop Spring Shift", EventDate = new DateTime(2026, 3, 15, 10, 0, 0), Location = "Grand River Hospital, 835 King St W", MaxVolunteers = 6 },

                // ---- Upcoming (next 2 weeks) ----
                new() { OrganizationId = organizations[2].OrganizationId, Title = "Saturday Morning Food Sorting", EventDate = now.AddDays(5).Date.AddHours(8).AddMinutes(30), Location = "50 Alpine Ct, Kitchener", MaxVolunteers = 30 },
                new() { OrganizationId = organizations[5].OrganizationId, Title = "Dog Walking & Kennel Enrichment", EventDate = now.AddDays(3).Date.AddHours(10), Location = "250 Riverbend Dr, Kitchener", MaxVolunteers = 8 },
                new() { OrganizationId = organizations[6].OrganizationId, Title = "Spring Trail Cleanup – Section B", EventDate = now.AddDays(8).Date.AddHours(9), Location = "Conestoga Greenway Trailhead, Blair Rd", MaxVolunteers = 15 },
                new() { OrganizationId = organizations[0].OrganizationId, Title = "Red Cross Blood Drive – Conestoga", EventDate = now.AddDays(10).Date.AddHours(11), Location = "Conestoga College, Doon Campus, Hall 1A04", MaxVolunteers = 12 },

                // ---- Future (planned) ----
                new() { OrganizationId = organizations[1].OrganizationId, Title = "Home Build #48 – Drywall & Paint", EventDate = now.AddDays(28).Date.AddHours(7).AddMinutes(30), Location = "162 Courtland Ave E, Kitchener", MaxVolunteers = 20 },
                new() { OrganizationId = organizations[3].OrganizationId, Title = "GRHF Annual Fundraising Gala", EventDate = now.AddDays(42).Date.AddHours(18), Location = "Bingemans, 425 Bingemans Centre Dr", MaxVolunteers = 45 },
                new() { OrganizationId = organizations[4].OrganizationId, Title = "Summer ESL Tutoring Program", EventDate = now.AddDays(55).Date.AddHours(9), Location = "KPL Main Branch, 85 Queen St N", MaxVolunteers = 20 },
                new() { OrganizationId = organizations[5].OrganizationId, Title = "Adoption Day – Kitchener City Hall", EventDate = now.AddDays(35).Date.AddHours(10), Location = "Kitchener City Hall, 200 King St W", MaxVolunteers = 18 },
            };
            context.Opportunities.AddRange(opportunities);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {opportunities.Count} Opportunities");

            // ── 6. VolunteerSkills ─────────────────────────────────────────────
            // Realistic: experienced volunteers have more skills; newer ones have fewer
            var volunteerSkills = new List<VolunteerSkill>
            {
                // Emily Chen (67.5h) – grad student, tutoring + first aid
                new() { VolunteerId = volunteers[0].VolunteerId, SkillId = skills[0].SkillId, ProficiencyLevel = "Intermediate", AcquiredDate = new DateTime(2024, 9, 15) },
                new() { VolunteerId = volunteers[0].VolunteerId, SkillId = skills[7].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2023, 5, 1) },

                // Marcus Johnson (23h) – has G license
                new() { VolunteerId = volunteers[1].VolunteerId, SkillId = skills[2].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2022, 8, 20) },
                new() { VolunteerId = volunteers[1].VolunteerId, SkillId = skills[5].SkillId, ProficiencyLevel = "Beginner", AcquiredDate = new DateTime(2025, 11, 1) },

                // Priya Sharma (104.5h) – very active, multiple skills
                new() { VolunteerId = volunteers[2].VolunteerId, SkillId = skills[0].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2023, 3, 10) },
                new() { VolunteerId = volunteers[2].VolunteerId, SkillId = skills[1].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2023, 6, 22) },
                new() { VolunteerId = volunteers[2].VolunteerId, SkillId = skills[3].SkillId, ProficiencyLevel = "Intermediate", AcquiredDate = new DateTime(2024, 1, 8) },

                // Liam MacKenzie (8.5h) – new, just carpentry
                new() { VolunteerId = volunteers[3].VolunteerId, SkillId = skills[5].SkillId, ProficiencyLevel = "Intermediate", AcquiredDate = new DateTime(2025, 6, 15) },

                // Fatima Al-Rashid (156h) – most experienced, many skills
                new() { VolunteerId = volunteers[4].VolunteerId, SkillId = skills[0].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2021, 11, 1) },
                new() { VolunteerId = volunteers[4].VolunteerId, SkillId = skills[1].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2022, 4, 18) },
                new() { VolunteerId = volunteers[4].VolunteerId, SkillId = skills[3].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2022, 1, 5) },
                new() { VolunteerId = volunteers[4].VolunteerId, SkillId = skills[4].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2020, 9, 1) },

                // Noah Beaulieu (41h) – student, social media
                new() { VolunteerId = volunteers[5].VolunteerId, SkillId = skills[6].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2024, 6, 1) },
                new() { VolunteerId = volunteers[5].VolunteerId, SkillId = skills[8].SkillId, ProficiencyLevel = "Intermediate", AcquiredDate = new DateTime(2024, 9, 10) },

                // Isabella Rivera (12h) – bilingual, tutoring
                new() { VolunteerId = volunteers[6].VolunteerId, SkillId = skills[4].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2015, 1, 1) },
                new() { VolunteerId = volunteers[6].VolunteerId, SkillId = skills[7].SkillId, ProficiencyLevel = "Intermediate", AcquiredDate = new DateTime(2025, 9, 15) },

                // Owen Taylor (0h) – brand new, no skills yet (realistic!)

                // Sophie Tremblay (89h) – bilingual coordinator type
                new() { VolunteerId = volunteers[8].VolunteerId, SkillId = skills[0].SkillId, ProficiencyLevel = "Intermediate", AcquiredDate = new DateTime(2024, 2, 20) },
                new() { VolunteerId = volunteers[8].VolunteerId, SkillId = skills[3].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2023, 7, 15) },
                new() { VolunteerId = volunteers[8].VolunteerId, SkillId = skills[4].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2019, 6, 1) },

                // James Okafor (34.5h) – driver + IT
                new() { VolunteerId = volunteers[9].VolunteerId, SkillId = skills[2].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2023, 4, 12) },
                new() { VolunteerId = volunteers[9].VolunteerId, SkillId = skills[8].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2022, 11, 1) },

                // Hannah Wolfe (19.5h) – animal handling + social media
                new() { VolunteerId = volunteers[10].VolunteerId, SkillId = skills[9].SkillId, ProficiencyLevel = "Intermediate", AcquiredDate = new DateTime(2025, 8, 1) },
                new() { VolunteerId = volunteers[10].VolunteerId, SkillId = skills[6].SkillId, ProficiencyLevel = "Beginner", AcquiredDate = new DateTime(2025, 10, 20) },

                // Raj Patel (52h) – food handler + driver
                new() { VolunteerId = volunteers[11].VolunteerId, SkillId = skills[1].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2024, 5, 30) },
                new() { VolunteerId = volunteers[11].VolunteerId, SkillId = skills[2].SkillId, ProficiencyLevel = "Advanced", AcquiredDate = new DateTime(2021, 7, 1) },
            };
            context.VolunteerSkills.AddRange(volunteerSkills);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {volunteerSkills.Count} VolunteerSkills");

            // ── 7. OpportunitySkills ──────────────────────────────────────────
            var opportunitySkills = new List<OpportunitySkill>
            {
                // Thanksgiving Food Drive Sorting – food handler mandatory
                new() { OpportunityId = opportunities[0].OpportunityId, SkillId = skills[1].SkillId, IsMandatory = true, MinimumLevel = "Beginner" },

                // Winter Storm Emergency Shelter – first aid mandatory, project coord preferred
                new() { OpportunityId = opportunities[1].OpportunityId, SkillId = skills[0].SkillId, IsMandatory = true, MinimumLevel = "Intermediate" },
                new() { OpportunityId = opportunities[1].OpportunityId, SkillId = skills[3].SkillId, IsMandatory = false, MinimumLevel = "Beginner" },

                // Home Build #47 – carpentry mandatory
                new() { OpportunityId = opportunities[2].OpportunityId, SkillId = skills[5].SkillId, IsMandatory = true, MinimumLevel = "Beginner" },

                // Newcomer Welcome – bilingual mandatory, tutoring preferred
                new() { OpportunityId = opportunities[3].OpportunityId, SkillId = skills[4].SkillId, IsMandatory = true, MinimumLevel = "Intermediate" },
                new() { OpportunityId = opportunities[3].OpportunityId, SkillId = skills[7].SkillId, IsMandatory = false, MinimumLevel = "Beginner" },

                // Saturday Food Sorting – food handler mandatory
                new() { OpportunityId = opportunities[5].OpportunityId, SkillId = skills[1].SkillId, IsMandatory = true, MinimumLevel = "Beginner" },

                // Dog Walking – animal handling mandatory
                new() { OpportunityId = opportunities[6].OpportunityId, SkillId = skills[9].SkillId, IsMandatory = true, MinimumLevel = "Beginner" },

                // Spring Trail Cleanup – driver's license preferred
                new() { OpportunityId = opportunities[7].OpportunityId, SkillId = skills[2].SkillId, IsMandatory = false, MinimumLevel = "Beginner" },

                // Red Cross Blood Drive – first aid mandatory
                new() { OpportunityId = opportunities[8].OpportunityId, SkillId = skills[0].SkillId, IsMandatory = true, MinimumLevel = "Advanced" },

                // Home Build #48 – carpentry mandatory
                new() { OpportunityId = opportunities[9].OpportunityId, SkillId = skills[5].SkillId, IsMandatory = true, MinimumLevel = "Beginner" },

                // GRHF Gala – project coord + social media
                new() { OpportunityId = opportunities[10].OpportunityId, SkillId = skills[3].SkillId, IsMandatory = true, MinimumLevel = "Intermediate" },
                new() { OpportunityId = opportunities[10].OpportunityId, SkillId = skills[6].SkillId, IsMandatory = false, MinimumLevel = "Beginner" },

                // ESL Tutoring – tutoring mandatory, bilingual preferred
                new() { OpportunityId = opportunities[11].OpportunityId, SkillId = skills[7].SkillId, IsMandatory = true, MinimumLevel = "Intermediate" },
                new() { OpportunityId = opportunities[11].OpportunityId, SkillId = skills[4].SkillId, IsMandatory = false, MinimumLevel = "Beginner" },

                // Adoption Day – animal handling + social media
                new() { OpportunityId = opportunities[12].OpportunityId, SkillId = skills[9].SkillId, IsMandatory = true, MinimumLevel = "Beginner" },
                new() { OpportunityId = opportunities[12].OpportunityId, SkillId = skills[6].SkillId, IsMandatory = false, MinimumLevel = "Beginner" },
            };
            context.OpportunitySkills.AddRange(opportunitySkills);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {opportunitySkills.Count} OpportunitySkills");

            // ── 8. Applications ───────────────────────────────────────────────
            // Realistic patterns:
            //   - Past events: mostly Approved, a couple Rejected
            //   - Upcoming events: mix of Pending and Approved
            //   - Future events: mostly Pending
            //   - Submission dates BEFORE the event, staggered naturally
            //   - Different coordinators modified different applications
            var applications = new List<Application>
            {
                // ── Past events ──

                // Thanksgiving Food Drive (Oct 11, 2025)
                new() { VolunteerId = volunteers[2].VolunteerId, OpportunityId = opportunities[0].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2025, 9, 28, 14, 22, 0),
                    CreatedByUserId = "afung@thefoodbank.ca", LastModifiedBy = "afung@thefoodbank.ca", LastModifiedAt = new DateTime(2025, 10, 1, 9, 15, 0) },
                new() { VolunteerId = volunteers[4].VolunteerId, OpportunityId = opportunities[0].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2025, 9, 30, 10, 5, 0),
                    CreatedByUserId = "afung@thefoodbank.ca", LastModifiedBy = "afung@thefoodbank.ca", LastModifiedAt = new DateTime(2025, 10, 2, 11, 30, 0) },
                new() { VolunteerId = volunteers[11].VolunteerId, OpportunityId = opportunities[0].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2025, 10, 3, 19, 44, 0),
                    CreatedByUserId = "admin@vsms.com", LastModifiedBy = "afung@thefoodbank.ca", LastModifiedAt = new DateTime(2025, 10, 5, 8, 0, 0) },

                // Winter Storm Emergency Shelter (Jan 18, 2026)
                new() { VolunteerId = volunteers[4].VolunteerId, OpportunityId = opportunities[1].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2026, 1, 15, 8, 12, 0),
                    CreatedByUserId = "sarah.mitchell@redcross.ca", LastModifiedBy = "sarah.mitchell@redcross.ca", LastModifiedAt = new DateTime(2026, 1, 15, 16, 45, 0) },
                new() { VolunteerId = volunteers[8].VolunteerId, OpportunityId = opportunities[1].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2026, 1, 16, 11, 30, 0),
                    CreatedByUserId = "sarah.mitchell@redcross.ca", LastModifiedBy = "sarah.mitchell@redcross.ca", LastModifiedAt = new DateTime(2026, 1, 16, 14, 20, 0) },
                new() { VolunteerId = volunteers[3].VolunteerId, OpportunityId = opportunities[1].OpportunityId,
                    Status = "Rejected", SubmissionDate = new DateTime(2026, 1, 17, 22, 8, 0),
                    CreatedByUserId = "admin@vsms.com", LastModifiedBy = "sarah.mitchell@redcross.ca", LastModifiedAt = new DateTime(2026, 1, 18, 7, 0, 0) },

                // Home Build #47 (Feb 8, 2026)
                new() { VolunteerId = volunteers[1].VolunteerId, OpportunityId = opportunities[2].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2026, 1, 25, 15, 33, 0),
                    CreatedByUserId = "carlos.medina@habitatwr.ca", LastModifiedBy = "carlos.medina@habitatwr.ca", LastModifiedAt = new DateTime(2026, 1, 28, 9, 10, 0) },
                new() { VolunteerId = volunteers[3].VolunteerId, OpportunityId = opportunities[2].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2026, 1, 30, 20, 17, 0),
                    CreatedByUserId = "admin@vsms.com", LastModifiedBy = "carlos.medina@habitatwr.ca", LastModifiedAt = new DateTime(2026, 2, 1, 10, 0, 0) },

                // Newcomer Welcome – Feb (Feb 22, 2026)
                new() { VolunteerId = volunteers[6].VolunteerId, OpportunityId = opportunities[3].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2026, 2, 10, 9, 45, 0),
                    CreatedByUserId = "npetrov@kwmc.on.ca", LastModifiedBy = "npetrov@kwmc.on.ca", LastModifiedAt = new DateTime(2026, 2, 12, 14, 30, 0) },
                new() { VolunteerId = volunteers[4].VolunteerId, OpportunityId = opportunities[3].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2026, 2, 14, 16, 20, 0),
                    CreatedByUserId = "npetrov@kwmc.on.ca", LastModifiedBy = "npetrov@kwmc.on.ca", LastModifiedAt = new DateTime(2026, 2, 15, 11, 0, 0) },

                // Hospital Gift Shop (Mar 15, 2026)
                new() { VolunteerId = volunteers[8].VolunteerId, OpportunityId = opportunities[4].OpportunityId,
                    Status = "Approved", SubmissionDate = new DateTime(2026, 3, 5, 13, 11, 0),
                    CreatedByUserId = "mosei@grhosp.on.ca", LastModifiedBy = "mosei@grhosp.on.ca", LastModifiedAt = new DateTime(2026, 3, 7, 10, 0, 0) },

                // ── Upcoming events ──

                // Saturday Food Sorting (upcoming ~5 days)
                new() { VolunteerId = volunteers[2].VolunteerId, OpportunityId = opportunities[5].OpportunityId,
                    Status = "Approved", SubmissionDate = now.AddDays(-12).Date.AddHours(10).AddMinutes(33),
                    CreatedByUserId = "afung@thefoodbank.ca", LastModifiedBy = "afung@thefoodbank.ca", LastModifiedAt = now.AddDays(-10).Date.AddHours(9) },
                new() { VolunteerId = volunteers[11].VolunteerId, OpportunityId = opportunities[5].OpportunityId,
                    Status = "Approved", SubmissionDate = now.AddDays(-9).Date.AddHours(18).AddMinutes(47),
                    CreatedByUserId = "admin@vsms.com", LastModifiedBy = "afung@thefoodbank.ca", LastModifiedAt = now.AddDays(-7).Date.AddHours(8).AddMinutes(30) },
                new() { VolunteerId = volunteers[7].VolunteerId, OpportunityId = opportunities[5].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-2).Date.AddHours(21).AddMinutes(14),
                    CreatedByUserId = "admin@vsms.com" },

                // Dog Walking (upcoming ~3 days)
                new() { VolunteerId = volunteers[10].VolunteerId, OpportunityId = opportunities[6].OpportunityId,
                    Status = "Approved", SubmissionDate = now.AddDays(-8).Date.AddHours(14).AddMinutes(5),
                    CreatedByUserId = "lbennett@kwhumane.com", LastModifiedBy = "lbennett@kwhumane.com", LastModifiedAt = now.AddDays(-6).Date.AddHours(11) },

                // Spring Trail Cleanup (upcoming ~8 days)
                new() { VolunteerId = volunteers[5].VolunteerId, OpportunityId = opportunities[7].OpportunityId,
                    Status = "Approved", SubmissionDate = now.AddDays(-15).Date.AddHours(12).AddMinutes(22),
                    CreatedByUserId = "tpark@conestogagreenway.ca", LastModifiedBy = "tpark@conestogagreenway.ca", LastModifiedAt = now.AddDays(-13).Date.AddHours(9) },
                new() { VolunteerId = volunteers[9].VolunteerId, OpportunityId = opportunities[7].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-4).Date.AddHours(7).AddMinutes(50),
                    CreatedByUserId = "admin@vsms.com" },
                new() { VolunteerId = volunteers[1].VolunteerId, OpportunityId = opportunities[7].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-1).Date.AddHours(20).AddMinutes(38),
                    CreatedByUserId = "admin@vsms.com" },

                // Red Cross Blood Drive (upcoming ~10 days)
                new() { VolunteerId = volunteers[0].VolunteerId, OpportunityId = opportunities[8].OpportunityId,
                    Status = "Approved", SubmissionDate = now.AddDays(-6).Date.AddHours(16).AddMinutes(9),
                    CreatedByUserId = "sarah.mitchell@redcross.ca", LastModifiedBy = "sarah.mitchell@redcross.ca", LastModifiedAt = now.AddDays(-5).Date.AddHours(10).AddMinutes(15) },
                new() { VolunteerId = volunteers[2].VolunteerId, OpportunityId = opportunities[8].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-3).Date.AddHours(11).AddMinutes(42),
                    CreatedByUserId = "admin@vsms.com" },

                // ── Future events ──

                // Home Build #48 (~28 days)
                new() { VolunteerId = volunteers[3].VolunteerId, OpportunityId = opportunities[9].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-3).Date.AddHours(9).AddMinutes(27),
                    CreatedByUserId = "carlos.medina@habitatwr.ca" },

                // GRHF Gala (~42 days)
                new() { VolunteerId = volunteers[8].VolunteerId, OpportunityId = opportunities[10].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-5).Date.AddHours(15).AddMinutes(3),
                    CreatedByUserId = "mosei@grhosp.on.ca" },
                new() { VolunteerId = volunteers[5].VolunteerId, OpportunityId = opportunities[10].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-2).Date.AddHours(13).AddMinutes(55),
                    CreatedByUserId = "admin@vsms.com" },

                // ESL Tutoring (~55 days)
                new() { VolunteerId = volunteers[0].VolunteerId, OpportunityId = opportunities[11].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-1).Date.AddHours(22).AddMinutes(10),
                    CreatedByUserId = "admin@vsms.com" },
                new() { VolunteerId = volunteers[6].VolunteerId, OpportunityId = opportunities[11].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddHours(-6),
                    CreatedByUserId = "npetrov@kwmc.on.ca" },

                // Adoption Day (~35 days)
                new() { VolunteerId = volunteers[10].VolunteerId, OpportunityId = opportunities[12].OpportunityId,
                    Status = "Pending", SubmissionDate = now.AddDays(-1).Date.AddHours(17).AddMinutes(32),
                    CreatedByUserId = "lbennett@kwhumane.com" },
            };
            context.Applications.AddRange(applications);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✅ {applications.Count} Applications");

            // ── Summary ───────────────────────────────────────────────────────
            var approved = applications.Count(a => a.Status == "Approved");
            var pending = applications.Count(a => a.Status == "Pending");
            var rejected = applications.Count(a => a.Status == "Rejected");

            var totalUsers = await context.Users.CountAsync();

            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║         🎉  Data Seed Complete               ║");
            Console.WriteLine("╠══════════════════════════════════════════════╣");
            Console.WriteLine($"║  User Accounts:         {totalUsers,3}                 ║");
            Console.WriteLine($"║  Organizations:           {await context.Organizations.CountAsync(),3}                 ║");
            Console.WriteLine($"║  Skills:                  {await context.Skills.CountAsync(),3}                 ║");
            Console.WriteLine($"║  Volunteers:              {await context.Volunteers.CountAsync(),3}                 ║");
            Console.WriteLine($"║  Coordinators:              {await context.Coordinators.CountAsync(),3}                 ║");
            Console.WriteLine($"║  Opportunities:            {await context.Opportunities.CountAsync(),3}                 ║");
            Console.WriteLine($"║  Applications:             {await context.Applications.CountAsync(),3}                 ║");
            Console.WriteLine($"║    ├─ Approved:            {approved,3}                 ║");
            Console.WriteLine($"║    ├─ Pending:             {pending,3}                 ║");
            Console.WriteLine($"║    └─ Rejected:              {rejected,3}                 ║");
            Console.WriteLine($"║  VolunteerSkills:          {await context.VolunteerSkills.CountAsync(),3}                 ║");
            Console.WriteLine($"║  OpportunitySkills:        {await context.OpportunitySkills.CountAsync(),3}                 ║");
            Console.WriteLine("╠══════════════════════════════════════════════╣");
            Console.WriteLine("║  🔑 Login Credentials                        ║");
            Console.WriteLine("║  Admin:       admin@vsms.com / Admin@123     ║");
            Console.WriteLine("║  Coordinator: sarah.mitchell@redcross.ca     ║");
            Console.WriteLine("║  Volunteer:   emily.chen@uwaterloo.ca        ║");
            Console.WriteLine("║  All seeded accounts use password: Pass@123  ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
        }

        private static string FindDatabasePath()
        {
            var dir = AppContext.BaseDirectory;
            for (int i = 0; i < 10; i++)
            {
                dir = Directory.GetParent(dir)?.FullName;
                if (dir == null) break;
                var candidate = Path.Combine(dir, "VSMS.Web2", "Data", "vsms.db");
                if (File.Exists(candidate)) return candidate;
            }
            var fallback = Path.Combine(Directory.GetCurrentDirectory(), "..", "VSMS.Web2", "Data", "vsms.db");
            return Path.GetFullPath(fallback);
        }
    }
}
