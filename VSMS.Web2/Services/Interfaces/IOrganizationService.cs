using VSMS.Web2.Models;

namespace VSMS.Web2.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
        Task<PaginatedList<Organization>> GetPaginatedOrganizationsAsync(int pageIndex, int pageSize);
        Task<Organization?> GetOrganizationByIdAsync(Guid id);
        Task AddOrganizationAsync(Organization organization);
        Task UpdateOrganizationAsync(Organization organization);
        Task DeleteOrganizationAsync(Guid id);
        bool OrganizationExists(Guid id);
    }
}
