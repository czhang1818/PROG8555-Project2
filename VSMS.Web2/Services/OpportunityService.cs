using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Services
{
    public class OpportunityService : IOpportunityService
    {
        private readonly AppDbContext _context;
        public OpportunityService(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Opportunity>> GetAllOpportunitiesAsync()
            => await _context.Opportunities.Include(o => o.Organization).AsNoTracking().ToListAsync();

        public async Task<PaginatedList<Opportunity>> GetPaginatedOpportunitiesAsync(int pageIndex, int pageSize)
            => await PaginatedList<Opportunity>.CreateAsync(
                _context.Opportunities.Include(o => o.Organization).AsNoTracking().OrderBy(o => o.EventDate), pageIndex, pageSize);

        public async Task<Opportunity?> GetOpportunityByIdAsync(Guid id)
            => await _context.Opportunities
                .Include(o => o.Organization)
                .Include(o => o.OpportunitySkills)
                .FirstOrDefaultAsync(m => m.OpportunityId == id);

        public async Task AddOpportunityAsync(Opportunity opportunity)
        { _context.Add(opportunity); await _context.SaveChangesAsync(); }

        public async Task UpdateOpportunityAsync(Opportunity opportunity)
        { _context.Update(opportunity); await _context.SaveChangesAsync(); }

        public async Task DeleteOpportunityAsync(Guid id)
        {
            var opp = await _context.Opportunities.FindAsync(id);
            if (opp != null) { _context.Opportunities.Remove(opp); await _context.SaveChangesAsync(); }
        }

        public async Task AddOrUpdateOpportunitySkillsAsync(Guid opportunityId, List<Guid> skillIds)
        {
            var existing = await _context.OpportunitySkills
                .Where(os => os.OpportunityId == opportunityId).ToListAsync();
            _context.OpportunitySkills.RemoveRange(existing);

            if (skillIds != null && skillIds.Any())
            {
                var newSkills = skillIds.Select(sid => new OpportunitySkill
                {
                    OpportunityId = opportunityId, SkillId = sid,
                    IsMandatory = false, MinimumLevel = "Beginner"
                });
                await _context.OpportunitySkills.AddRangeAsync(newSkills);
            }
            await _context.SaveChangesAsync();
        }

        public bool OpportunityExists(Guid id)
            => _context.Opportunities.Any(e => e.OpportunityId == id);
    }
}
