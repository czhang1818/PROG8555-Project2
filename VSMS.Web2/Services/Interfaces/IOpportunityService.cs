using VSMS.Web2.Models;

namespace VSMS.Web2.Services.Interfaces
{
    public interface IOpportunityService
    {
        Task<IEnumerable<Opportunity>> GetAllOpportunitiesAsync();
        Task<PaginatedList<Opportunity>> GetPaginatedOpportunitiesAsync(int pageIndex, int pageSize);
        Task<Opportunity?> GetOpportunityByIdAsync(Guid id);
        Task AddOpportunityAsync(Opportunity opportunity);
        Task UpdateOpportunityAsync(Opportunity opportunity);
        Task DeleteOpportunityAsync(Guid id);
        Task AddOrUpdateOpportunitySkillsAsync(Guid opportunityId, List<Guid> skillIds);
        bool OpportunityExists(Guid id);
    }
}
