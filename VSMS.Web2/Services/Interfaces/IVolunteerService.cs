using VSMS.Web2.Models;

namespace VSMS.Web2.Services.Interfaces
{
    public interface IVolunteerService
    {
        Task<IEnumerable<Volunteer>> GetAllVolunteersAsync();
        Task<PaginatedList<Volunteer>> GetPaginatedVolunteersAsync(int pageIndex, int pageSize);
        Task<Volunteer?> GetVolunteerByIdAsync(Guid id);
        Task AddVolunteerAsync(Volunteer volunteer);
        Task UpdateVolunteerAsync(Volunteer volunteer);
        Task DeleteVolunteerAsync(Guid id);
        Task AddOrUpdateVolunteerSkillsAsync(Guid volunteerId, List<Guid> skillIds);
        bool VolunteerExists(Guid id);
    }
}
