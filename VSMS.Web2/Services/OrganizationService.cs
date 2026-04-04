using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly AppDbContext _context;
        public OrganizationService(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
            => await _context.Organizations.AsNoTracking().ToListAsync();

        public async Task<PaginatedList<Organization>> GetPaginatedOrganizationsAsync(int pageIndex, int pageSize)
            => await PaginatedList<Organization>.CreateAsync(
                _context.Organizations.AsNoTracking().OrderBy(o => o.Name), pageIndex, pageSize);

        public async Task<Organization?> GetOrganizationByIdAsync(Guid id)
            => await _context.Organizations.FirstOrDefaultAsync(m => m.OrganizationId == id);

        public async Task AddOrganizationAsync(Organization organization)
        { _context.Add(organization); await _context.SaveChangesAsync(); }

        public async Task UpdateOrganizationAsync(Organization organization)
        { _context.Update(organization); await _context.SaveChangesAsync(); }

        public async Task DeleteOrganizationAsync(Guid id)
        {
            var org = await _context.Organizations.FindAsync(id);
            if (org != null) { _context.Organizations.Remove(org); await _context.SaveChangesAsync(); }
        }

        public bool OrganizationExists(Guid id)
            => _context.Organizations.Any(e => e.OrganizationId == id);
    }
}
