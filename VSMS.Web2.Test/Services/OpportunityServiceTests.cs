using VSMS.Web2.Models;
using VSMS.Web2.Services;
using VSMS.Web2.Test.Fixtures;

namespace VSMS.Web2.Test.Services
{
    public class OpportunityServiceTests : IDisposable
    {
        private readonly DatabaseFixture _fixture;
        private readonly OpportunityService _service;

        public OpportunityServiceTests()
        {
            _fixture = new DatabaseFixture();
            _service = new OpportunityService(_fixture.Context);
        }

        private Organization CreateTestOrganization(string name = "Test Org")
        {
            var org = new Organization { Name = name, ContactEmail = $"{name.Replace(" ", "").ToLower()}@test.com" };
            _fixture.Context.Organizations.Add(org);
            _fixture.Context.SaveChanges();
            return org;
        }

        [Fact]
        public async Task GetAllOpportunities_ReturnsEmpty_Initially()
        {
            var result = await _service.GetAllOpportunitiesAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddOpportunity_And_GetById_ReturnsOpportunity()
        {
            var org = CreateTestOrganization();
            var opp = new Opportunity
            {
                OrganizationId = org.OrganizationId,
                Title = "Community Cleanup",
                EventDate = new DateTime(2026, 6, 15),
                Location = "Central Park",
                MaxVolunteers = 50
            };

            await _service.AddOpportunityAsync(opp);

            using var readContext = _fixture.CreateNewContext();
            var readService = new OpportunityService(readContext);
            var result = await readService.GetOpportunityByIdAsync(opp.OpportunityId);

            Assert.NotNull(result);
            Assert.Equal("Community Cleanup", result.Title);
            Assert.Equal("Central Park", result.Location);
            Assert.Equal(50, result.MaxVolunteers);
            Assert.NotNull(result.Organization);
        }

        [Fact]
        public async Task AddMultiple_And_GetAll_ReturnsAllWithOrganization()
        {
            var org = CreateTestOrganization();
            await _service.AddOpportunityAsync(new Opportunity { OrganizationId = org.OrganizationId, Title = "A", Location = "L1", MaxVolunteers = 10 });
            await _service.AddOpportunityAsync(new Opportunity { OrganizationId = org.OrganizationId, Title = "B", Location = "L2", MaxVolunteers = 20 });

            using var readContext = _fixture.CreateNewContext();
            var readService = new OpportunityService(readContext);
            var result = await readService.GetAllOpportunitiesAsync();

            Assert.Equal(2, result.Count());
            Assert.All(result, opp => Assert.NotNull(opp.Organization));
        }

        [Fact]
        public async Task UpdateOpportunity_ModifiesEntity()
        {
            var org = CreateTestOrganization();
            var opp = new Opportunity { OrganizationId = org.OrganizationId, Title = "Old", Location = "Old Place", MaxVolunteers = 5 };
            await _service.AddOpportunityAsync(opp);

            opp.Title = "New Title";
            opp.MaxVolunteers = 100;
            await _service.UpdateOpportunityAsync(opp);

            using var readContext = _fixture.CreateNewContext();
            var readService = new OpportunityService(readContext);
            var result = await readService.GetOpportunityByIdAsync(opp.OpportunityId);

            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.Equal(100, result.MaxVolunteers);
        }

        [Fact]
        public async Task DeleteOpportunity_RemovesEntity()
        {
            var org = CreateTestOrganization();
            var opp = new Opportunity { OrganizationId = org.OrganizationId, Title = "Del", Location = "X", MaxVolunteers = 1 };
            await _service.AddOpportunityAsync(opp);

            await _service.DeleteOpportunityAsync(opp.OpportunityId);

            using var readContext = _fixture.CreateNewContext();
            var readService = new OpportunityService(readContext);
            var result = await readService.GetOpportunityByIdAsync(opp.OpportunityId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPaginated_ReturnsCorrectPage()
        {
            var org = CreateTestOrganization();
            for (int i = 0; i < 12; i++)
            {
                await _service.AddOpportunityAsync(new Opportunity
                {
                    OrganizationId = org.OrganizationId,
                    Title = $"Opp {i:D2}",
                    Location = $"Location {i}",
                    MaxVolunteers = i + 1,
                    EventDate = DateTime.Now.AddDays(i)
                });
            }

            using var readContext = _fixture.CreateNewContext();
            var readService = new OpportunityService(readContext);

            var page1 = await readService.GetPaginatedOpportunitiesAsync(1, 5);
            Assert.Equal(5, page1.Count);
            Assert.Equal(12, page1.TotalCount);
        }

        [Fact]
        public async Task AddOrUpdateOpportunitySkills_AssignsSkills()
        {
            var org = CreateTestOrganization();
            var opp = new Opportunity { OrganizationId = org.OrganizationId, Title = "Skilled", Location = "Here", MaxVolunteers = 10 };
            await _service.AddOpportunityAsync(opp);

            var skill1 = new Skill { Name = "Leadership", Category = "Soft" };
            var skill2 = new Skill { Name = "Teamwork", Category = "Soft" };
            _fixture.Context.Skills.AddRange(skill1, skill2);
            await _fixture.Context.SaveChangesAsync();

            await _service.AddOrUpdateOpportunitySkillsAsync(opp.OpportunityId, new List<Guid> { skill1.SkillId, skill2.SkillId });

            using var readContext = _fixture.CreateNewContext();
            var readService = new OpportunityService(readContext);
            var result = await readService.GetOpportunityByIdAsync(opp.OpportunityId);

            Assert.NotNull(result);
            Assert.NotNull(result.OpportunitySkills);
            Assert.Equal(2, result.OpportunitySkills.Count);
        }

        [Fact]
        public async Task AddOrUpdateOpportunitySkills_ReplacesExisting()
        {
            var org = CreateTestOrganization();
            var opp = new Opportunity { OrganizationId = org.OrganizationId, Title = "Replace", Location = "There", MaxVolunteers = 5 };
            await _service.AddOpportunityAsync(opp);

            var s1 = new Skill { Name = "S1", Category = "X" };
            var s2 = new Skill { Name = "S2", Category = "X" };
            _fixture.Context.Skills.AddRange(s1, s2);
            await _fixture.Context.SaveChangesAsync();

            await _service.AddOrUpdateOpportunitySkillsAsync(opp.OpportunityId, new List<Guid> { s1.SkillId });
            await _service.AddOrUpdateOpportunitySkillsAsync(opp.OpportunityId, new List<Guid> { s2.SkillId });

            using var readContext = _fixture.CreateNewContext();
            var readService = new OpportunityService(readContext);
            var result = await readService.GetOpportunityByIdAsync(opp.OpportunityId);

            Assert.NotNull(result);
            Assert.NotNull(result.OpportunitySkills);
            Assert.Single(result.OpportunitySkills);
            Assert.Equal(s2.SkillId, result.OpportunitySkills.First().SkillId);
        }

        [Fact]
        public void OpportunityExists_ReturnsTrueForExisting()
        {
            var org = CreateTestOrganization();
            var opp = new Opportunity { OrganizationId = org.OrganizationId, Title = "Exists", Location = "X", MaxVolunteers = 1 };
            _fixture.Context.Opportunities.Add(opp);
            _fixture.Context.SaveChanges();

            Assert.True(_service.OpportunityExists(opp.OpportunityId));
            Assert.False(_service.OpportunityExists(Guid.NewGuid()));
        }

        public void Dispose() => _fixture.Dispose();
    }
}
