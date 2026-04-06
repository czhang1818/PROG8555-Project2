using VSMS.Web2.Models;
using VSMS.Web2.Services;
using VSMS.Web2.Test.Fixtures;

namespace VSMS.Web2.Test.Services
{
    public class CoordinatorServiceTests : IDisposable
    {
        private readonly DatabaseFixture _fixture;
        private readonly CoordinatorService _service;

        public CoordinatorServiceTests()
        {
            _fixture = new DatabaseFixture();
            _service = new CoordinatorService(_fixture.Context);
        }

        private Organization CreateTestOrganization(string name = "Test Org")
        {
            var org = new Organization { Name = name, ContactEmail = $"{name.Replace(" ", "").ToLower()}@test.com" };
            _fixture.Context.Organizations.Add(org);
            _fixture.Context.SaveChanges();
            return org;
        }

        [Fact]
        public async Task GetAllCoordinators_ReturnsEmpty_Initially()
        {
            var result = await _service.GetAllCoordinatorsAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddCoordinator_And_GetById_ReturnsCoordinator()
        {
            var org = CreateTestOrganization();
            var coord = new Coordinator
            {
                OrganizationId = org.OrganizationId,
                Name = "Jane Smith",
                JobTitle = "Program Manager",
                Email = "jane@org.com",
                PhoneNumber = "555-0202"
            };

            await _service.AddCoordinatorAsync(coord);

            using var readContext = _fixture.CreateNewContext();
            var readService = new CoordinatorService(readContext);
            var result = await readService.GetCoordinatorByIdAsync(coord.CoordinatorId);

            Assert.NotNull(result);
            Assert.Equal("Jane Smith", result.Name);
            Assert.Equal("Program Manager", result.JobTitle);
        }

        [Fact]
        public async Task AddMultiple_And_GetAll_ReturnsAll()
        {
            var org = CreateTestOrganization();
            await _service.AddCoordinatorAsync(new Coordinator { OrganizationId = org.OrganizationId, Name = "C1", JobTitle = "J1", Email = "c1@t.com", PhoneNumber = "111" });
            await _service.AddCoordinatorAsync(new Coordinator { OrganizationId = org.OrganizationId, Name = "C2", JobTitle = "J2", Email = "c2@t.com", PhoneNumber = "222" });

            using var readContext = _fixture.CreateNewContext();
            var readService = new CoordinatorService(readContext);
            var result = await readService.GetAllCoordinatorsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateCoordinator_ModifiesEntity()
        {
            var org = CreateTestOrganization();
            var coord = new Coordinator { OrganizationId = org.OrganizationId, Name = "Before", JobTitle = "Old", Email = "b@t.com", PhoneNumber = "000" };
            await _service.AddCoordinatorAsync(coord);

            coord.Name = "After";
            coord.JobTitle = "New Title";
            await _service.UpdateCoordinatorAsync(coord);

            using var readContext = _fixture.CreateNewContext();
            var readService = new CoordinatorService(readContext);
            var result = await readService.GetCoordinatorByIdAsync(coord.CoordinatorId);

            Assert.NotNull(result);
            Assert.Equal("After", result.Name);
            Assert.Equal("New Title", result.JobTitle);
        }

        [Fact]
        public async Task DeleteCoordinator_RemovesEntity()
        {
            var org = CreateTestOrganization();
            var coord = new Coordinator { OrganizationId = org.OrganizationId, Name = "Del", JobTitle = "J", Email = "d@t.com", PhoneNumber = "000" };
            await _service.AddCoordinatorAsync(coord);

            await _service.DeleteCoordinatorAsync(coord.CoordinatorId);

            using var readContext = _fixture.CreateNewContext();
            var readService = new CoordinatorService(readContext);
            var result = await readService.GetCoordinatorByIdAsync(coord.CoordinatorId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPaginated_ReturnsCorrectPage()
        {
            var org = CreateTestOrganization();
            for (int i = 0; i < 7; i++)
            {
                await _service.AddCoordinatorAsync(new Coordinator
                {
                    OrganizationId = org.OrganizationId,
                    Name = $"Coord {i:D2}",
                    JobTitle = "Coordinator",
                    Email = $"c{i}@t.com",
                    PhoneNumber = $"555-{i:D4}"
                });
            }

            using var readContext = _fixture.CreateNewContext();
            var readService = new CoordinatorService(readContext);

            var page1 = await readService.GetPaginatedCoordinatorsAsync(1, 5);
            Assert.Equal(5, page1.Count);
            Assert.Equal(7, page1.TotalCount);
            Assert.True(page1.HasNextPage);

            var page2 = await readService.GetPaginatedCoordinatorsAsync(2, 5);
            Assert.Equal(2, page2.Count);
            Assert.False(page2.HasNextPage);
        }

        [Fact]
        public void CoordinatorExists_ReturnsTrueForExisting()
        {
            var org = CreateTestOrganization();
            var coord = new Coordinator { OrganizationId = org.OrganizationId, Name = "Exists", JobTitle = "J", Email = "e@t.com", PhoneNumber = "000" };
            _fixture.Context.Coordinators.Add(coord);
            _fixture.Context.SaveChanges();

            Assert.True(_service.CoordinatorExists(coord.CoordinatorId));
            Assert.False(_service.CoordinatorExists(Guid.NewGuid()));
        }

        public void Dispose() => _fixture.Dispose();
    }
}
