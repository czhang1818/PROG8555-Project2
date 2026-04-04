using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Models;

namespace VSMS.Web2.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets
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
            base.OnModelCreating(modelBuilder); // Required for Identity tables

            // Composite primary key for VolunteerSkill
            modelBuilder.Entity<VolunteerSkill>()
                .HasKey(vs => new { vs.VolunteerId, vs.SkillId });

            modelBuilder.Entity<VolunteerSkill>()
                .HasOne(vs => vs.Volunteer)
                .WithMany(v => v.VolunteerSkills)
                .HasForeignKey(vs => vs.VolunteerId);

            modelBuilder.Entity<VolunteerSkill>()
                .HasOne(vs => vs.Skill)
                .WithMany(s => s.VolunteerSkills)
                .HasForeignKey(vs => vs.SkillId);

            // Composite primary key for OpportunitySkill
            modelBuilder.Entity<OpportunitySkill>()
                .HasKey(os => new { os.OpportunityId, os.SkillId });

            modelBuilder.Entity<OpportunitySkill>()
                .HasOne(os => os.Opportunity)
                .WithMany(o => o.OpportunitySkills)
                .HasForeignKey(os => os.OpportunityId);

            modelBuilder.Entity<OpportunitySkill>()
                .HasOne(os => os.Skill)
                .WithMany(s => s.OpportunitySkills)
                .HasForeignKey(os => os.SkillId);
        }
    }
}
