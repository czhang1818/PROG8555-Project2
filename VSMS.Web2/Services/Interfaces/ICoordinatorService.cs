using VSMS.Web2.Models;

namespace VSMS.Web2.Services.Interfaces
{
    public interface ICoordinatorService
    {
        Task<IEnumerable<Coordinator>> GetAllCoordinatorsAsync();
        Task<PaginatedList<Coordinator>> GetPaginatedCoordinatorsAsync(int pageIndex, int pageSize);
        Task<Coordinator?> GetCoordinatorByIdAsync(Guid id);
        Task AddCoordinatorAsync(Coordinator coordinator);
        Task UpdateCoordinatorAsync(Coordinator coordinator);
        Task DeleteCoordinatorAsync(Guid id);
        bool CoordinatorExists(Guid id);
    }
}
