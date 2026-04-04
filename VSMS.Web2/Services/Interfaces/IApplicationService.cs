using VSMS.Web2.Models;

namespace VSMS.Web2.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<Application>> GetAllApplicationsAsync();
        Task<PaginatedList<Application>> GetPaginatedApplicationsAsync(int pageIndex, int pageSize);
        Task<Application?> GetApplicationByIdAsync(Guid id);
        Task AddApplicationAsync(Application application);
        Task UpdateApplicationAsync(Application application);
        Task DeleteApplicationAsync(Guid id);
        bool ApplicationExists(Guid id);
    }
}
