using VSMS.Web2.Models;
using VSMS.Web2.Services;
using VSMS.Web2.Test.Fixtures;

namespace VSMS.Web2.Test.Services
{
    public class ApplicationServiceTests : IDisposable
    {
        private readonly DatabaseFixture _fixture;
        private readonly ApplicationService _service;

        public ApplicationServiceTests()
        {
            _fixture = new DatabaseFixture();
            _service = new ApplicationService(_fixture.Context);
        }

        private (Volunteer volunteer, Opportunity opportunity) CreateTestData()
        {
            var org = new Organization { Name = "App Test Org", ContactEmail = "ato@test.com" };
            _fixture.Context.Organizations.Add(org);

            var vol = new Volunteer { Name = "App Vol", Email = "av@t.com", PhoneNumber = "000" };
            _fixture.Context.Volunteers.Add(vol);

            var opp = new Opportunity
            {
                OrganizationId = org.OrganizationId,
                Title = "Test Opportunity",
                Location = "Test Location",
                MaxVolunteers = 20
            };
            _fixture.Context.Opportunities.Add(opp);
            _fixture.Context.SaveChanges();

            return (vol, opp);
        }

        [Fact]
        public async Task GetAllApplications_ReturnsEmpty_Initially()
        {
            var result = await _service.GetAllApplicationsAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddApplication_And_GetById_ReturnsApplicationWithNavigationProperties()
        {
            var (vol, opp) = CreateTestData();
            var app = new Application
            {
                VolunteerId = vol.VolunteerId,
                OpportunityId = opp.OpportunityId,
                Status = "Pending",
                CreatedByUserId = "admin@vsms.com"
            };

            await _service.AddApplicationAsync(app);

            using var readContext = _fixture.CreateNewContext();
            var readService = new ApplicationService(readContext);
            var result = await readService.GetApplicationByIdAsync(app.AppId);

            Assert.NotNull(result);
            Assert.Equal("Pending", result.Status);
            Assert.Equal("admin@vsms.com", result.CreatedByUserId);
            Assert.NotNull(result.Volunteer);
            Assert.NotNull(result.Opportunity);
        }

        [Fact]
        public async Task AddMultiple_And_GetAll_ReturnsAll()
        {
            var (vol, opp) = CreateTestData();
            await _service.AddApplicationAsync(new Application { VolunteerId = vol.VolunteerId, OpportunityId = opp.OpportunityId, Status = "Pending" });
            await _service.AddApplicationAsync(new Application { VolunteerId = vol.VolunteerId, OpportunityId = opp.OpportunityId, Status = "Approved" });

            using var readContext = _fixture.CreateNewContext();
            var readService = new ApplicationService(readContext);
            var result = await readService.GetAllApplicationsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateApplication_ModifiesStatusAndAuditFields()
        {
            var (vol, opp) = CreateTestData();
            var app = new Application
            {
                VolunteerId = vol.VolunteerId,
                OpportunityId = opp.OpportunityId,
                Status = "Pending",
                CreatedByUserId = "admin@vsms.com"
            };
            await _service.AddApplicationAsync(app);

            // Simulate controller edit action
            app.Status = "Approved";
            app.LastModifiedBy = "coordinator@vsms.com";
            app.LastModifiedAt = DateTime.UtcNow;
            await _service.UpdateApplicationAsync(app);

            using var readContext = _fixture.CreateNewContext();
            var readService = new ApplicationService(readContext);
            var result = await readService.GetApplicationByIdAsync(app.AppId);

            Assert.NotNull(result);
            Assert.Equal("Approved", result.Status);
            Assert.Equal("admin@vsms.com", result.CreatedByUserId);
            Assert.Equal("coordinator@vsms.com", result.LastModifiedBy);
            Assert.NotNull(result.LastModifiedAt);
        }

        [Fact]
        public async Task DeleteApplication_RemovesEntity()
        {
            var (vol, opp) = CreateTestData();
            var app = new Application { VolunteerId = vol.VolunteerId, OpportunityId = opp.OpportunityId };
            await _service.AddApplicationAsync(app);

            await _service.DeleteApplicationAsync(app.AppId);

            using var readContext = _fixture.CreateNewContext();
            var readService = new ApplicationService(readContext);
            var result = await readService.GetApplicationByIdAsync(app.AppId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPaginated_ReturnsCorrectPage_OrderedBySubmissionDateDesc()
        {
            var (vol, opp) = CreateTestData();
            for (int i = 0; i < 11; i++)
            {
                await _service.AddApplicationAsync(new Application
                {
                    VolunteerId = vol.VolunteerId,
                    OpportunityId = opp.OpportunityId,
                    Status = i % 3 == 0 ? "Approved" : (i % 3 == 1 ? "Pending" : "Rejected"),
                    SubmissionDate = DateTime.UtcNow.AddDays(-i)
                });
            }

            using var readContext = _fixture.CreateNewContext();
            var readService = new ApplicationService(readContext);

            var page1 = await readService.GetPaginatedApplicationsAsync(1, 5);
            Assert.Equal(5, page1.Count);
            Assert.Equal(11, page1.TotalCount);
            Assert.True(page1.HasNextPage);

            var page3 = await readService.GetPaginatedApplicationsAsync(3, 5);
            Assert.Single(page3);
            Assert.False(page3.HasNextPage);
        }

        [Fact]
        public async Task AuditFields_PreservedAcrossUpdates()
        {
            var (vol, opp) = CreateTestData();
            var app = new Application
            {
                VolunteerId = vol.VolunteerId,
                OpportunityId = opp.OpportunityId,
                Status = "Pending",
                CreatedByUserId = "creator@vsms.com"
            };
            await _service.AddApplicationAsync(app);

            // First update
            app.Status = "Approved";
            app.LastModifiedBy = "editor1@vsms.com";
            app.LastModifiedAt = new DateTime(2026, 1, 15);
            await _service.UpdateApplicationAsync(app);

            // Second update
            app.Status = "Rejected";
            app.LastModifiedBy = "editor2@vsms.com";
            app.LastModifiedAt = new DateTime(2026, 2, 20);
            await _service.UpdateApplicationAsync(app);

            using var readContext = _fixture.CreateNewContext();
            var readService = new ApplicationService(readContext);
            var result = await readService.GetApplicationByIdAsync(app.AppId);

            Assert.NotNull(result);
            Assert.Equal("Rejected", result.Status);
            Assert.Equal("creator@vsms.com", result.CreatedByUserId); // Original creator preserved
            Assert.Equal("editor2@vsms.com", result.LastModifiedBy);  // Latest modifier
        }

        [Fact]
        public void ApplicationExists_ReturnsTrueForExisting()
        {
            var (vol, opp) = CreateTestData();
            var app = new Application { VolunteerId = vol.VolunteerId, OpportunityId = opp.OpportunityId };
            _fixture.Context.Applications.Add(app);
            _fixture.Context.SaveChanges();

            Assert.True(_service.ApplicationExists(app.AppId));
            Assert.False(_service.ApplicationExists(Guid.NewGuid()));
        }

        public void Dispose() => _fixture.Dispose();
    }
}
