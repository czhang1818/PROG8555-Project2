using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly AppDbContext _context;
        public VolunteerService(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Volunteer>> GetAllVolunteersAsync()
            => await _context.Volunteers.AsNoTracking().ToListAsync();

        public async Task<PaginatedList<Volunteer>> GetPaginatedVolunteersAsync(int pageIndex, int pageSize)
            => await PaginatedList<Volunteer>.CreateAsync(
                _context.Volunteers.AsNoTracking().OrderBy(v => v.Name), pageIndex, pageSize);

        public async Task<Volunteer?> GetVolunteerByIdAsync(Guid id)
            => await _context.Volunteers
                .Include(v => v.VolunteerSkills)
                .FirstOrDefaultAsync(m => m.VolunteerId == id);

        public async Task AddVolunteerAsync(Volunteer volunteer)
        { _context.Add(volunteer); await _context.SaveChangesAsync(); }

        public async Task UpdateVolunteerAsync(Volunteer volunteer)
        { _context.Update(volunteer); await _context.SaveChangesAsync(); }

        public async Task DeleteVolunteerAsync(Guid id)
        {
            var vol = await _context.Volunteers.FindAsync(id);
            if (vol != null) { _context.Volunteers.Remove(vol); await _context.SaveChangesAsync(); }
        }

        public async Task AddOrUpdateVolunteerSkillsAsync(Guid volunteerId, List<Guid> skillIds)
        {
            var existing = await _context.VolunteerSkills
                .Where(vs => vs.VolunteerId == volunteerId).ToListAsync();
            _context.VolunteerSkills.RemoveRange(existing);

            if (skillIds != null && skillIds.Any())
            {
                var newSkills = skillIds.Select(sid => new VolunteerSkill
                {
                    VolunteerId = volunteerId, SkillId = sid,
                    AcquiredDate = DateTime.UtcNow, ProficiencyLevel = "Beginner"
                });
                await _context.VolunteerSkills.AddRangeAsync(newSkills);
            }
            await _context.SaveChangesAsync();
        }

        public bool VolunteerExists(Guid id)
            => _context.Volunteers.Any(e => e.VolunteerId == id);
    }
}
