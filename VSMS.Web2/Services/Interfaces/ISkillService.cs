using VSMS.Web2.Models;

namespace VSMS.Web2.Services.Interfaces
{
    public interface ISkillService
    {
        Task<IEnumerable<Skill>> GetAllSkillsAsync();
        Task<PaginatedList<Skill>> GetPaginatedSkillsAsync(int pageIndex, int pageSize);
        Task<Skill?> GetSkillByIdAsync(Guid id);
        Task AddSkillAsync(Skill skill);
        Task UpdateSkillAsync(Skill skill);
        Task DeleteSkillAsync(Guid id);
        bool SkillExists(Guid id);
    }
}
