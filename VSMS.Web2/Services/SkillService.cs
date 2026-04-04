using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Services
{
    public class SkillService : ISkillService
    {
        private readonly AppDbContext _context;
        public SkillService(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Skill>> GetAllSkillsAsync()
            => await _context.Skills.AsNoTracking().ToListAsync();

        public async Task<PaginatedList<Skill>> GetPaginatedSkillsAsync(int pageIndex, int pageSize)
            => await PaginatedList<Skill>.CreateAsync(
                _context.Skills.AsNoTracking().OrderBy(s => s.Name), pageIndex, pageSize);

        public async Task<Skill?> GetSkillByIdAsync(Guid id)
            => await _context.Skills.FirstOrDefaultAsync(m => m.SkillId == id);

        public async Task AddSkillAsync(Skill skill)
        { _context.Add(skill); await _context.SaveChangesAsync(); }

        public async Task UpdateSkillAsync(Skill skill)
        { _context.Update(skill); await _context.SaveChangesAsync(); }

        public async Task DeleteSkillAsync(Guid id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill != null) { _context.Skills.Remove(skill); await _context.SaveChangesAsync(); }
        }

        public bool SkillExists(Guid id)
            => _context.Skills.Any(e => e.SkillId == id);
    }
}
