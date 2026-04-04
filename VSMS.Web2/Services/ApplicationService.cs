using Microsoft.EntityFrameworkCore;
using VSMS.Web2.Data;
using VSMS.Web2.Models;
using VSMS.Web2.Services.Interfaces;

namespace VSMS.Web2.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly AppDbContext _context;
        public ApplicationService(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
            => await _context.Applications
                .Include(a => a.Opportunity)
                .Include(a => a.Volunteer)
                .AsNoTracking()
                .ToListAsync();

        public async Task<PaginatedList<Application>> GetPaginatedApplicationsAsync(int pageIndex, int pageSize)
            => await PaginatedList<Application>.CreateAsync(
                _context.Applications
                    .Include(a => a.Opportunity)
                    .Include(a => a.Volunteer)
                    .AsNoTracking()
                    .OrderByDescending(a => a.SubmissionDate), pageIndex, pageSize);

        public async Task<Application?> GetApplicationByIdAsync(Guid id)
            => await _context.Applications
                .Include(a => a.Opportunity)
                .Include(a => a.Volunteer)
                .FirstOrDefaultAsync(m => m.AppId == id);

        public async Task AddApplicationAsync(Application application)
        { _context.Add(application); await _context.SaveChangesAsync(); }

        public async Task UpdateApplicationAsync(Application application)
        { _context.Update(application); await _context.SaveChangesAsync(); }

        public async Task DeleteApplicationAsync(Guid id)
        {
            var app = await _context.Applications.FindAsync(id);
            if (app != null) { _context.Applications.Remove(app); await _context.SaveChangesAsync(); }
        }

        public bool ApplicationExists(Guid id)
            => _context.Applications.Any(e => e.AppId == id);
    }
}
