using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Services
{
    public class CoordinatorService : ICoordinatorService
    {
        private readonly AppDbContext _context;
        public CoordinatorService(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Coordinator>> GetAllCoordinatorsAsync()
            => await _context.Coordinators.Include(c => c.Organization).AsNoTracking().ToListAsync();

        public async Task<PaginatedList<Coordinator>> GetPaginatedCoordinatorsAsync(int pageIndex, int pageSize)
            => await PaginatedList<Coordinator>.CreateAsync(
                _context.Coordinators.Include(c => c.Organization).AsNoTracking().OrderBy(c => c.Name), pageIndex, pageSize);

        public async Task<Coordinator?> GetCoordinatorByIdAsync(Guid id)
            => await _context.Coordinators
                .Include(c => c.Organization)
                .FirstOrDefaultAsync(m => m.CoordinatorId == id);

        public async Task AddCoordinatorAsync(Coordinator coordinator)
        { _context.Add(coordinator); await _context.SaveChangesAsync(); }

        public async Task UpdateCoordinatorAsync(Coordinator coordinator)
        { _context.Update(coordinator); await _context.SaveChangesAsync(); }

        public async Task DeleteCoordinatorAsync(Guid id)
        {
            var coord = await _context.Coordinators.FindAsync(id);
            if (coord != null) { _context.Coordinators.Remove(coord); await _context.SaveChangesAsync(); }
        }

        public bool CoordinatorExists(Guid id)
            => _context.Coordinators.Any(e => e.CoordinatorId == id);
    }
}
