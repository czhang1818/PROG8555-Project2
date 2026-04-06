using VSMS.Web2.Models;
using VSMS.Web2.Services;
using VSMS.Web2.Test.Fixtures;

namespace VSMS.Web2.Test.Services
{
    public class OrganizationServiceTests : IDisposable
    {
        private readonly DatabaseFixture _fixture;
        private readonly OrganizationService _service;

        public OrganizationServiceTests()
        {
            _fixture = new DatabaseFixture();
            _service = new OrganizationService(_fixture.Context);
        }

        [Fact]
        public async Task GetAllOrganizations_ReturnsEmpty_Initially()
        {
            var result = await _service.GetAllOrganizationsAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddOrganization_And_GetById_ReturnsOrganization()
        {
            var org = new Organization
            {
                Name = "Test Organization",
                ContactEmail = "test@org.com",
                Website = "https://testorg.com",
                IsVerified = true
            };

            await _service.AddOrganizationAsync(org);

            // Read from a fresh context to avoid caching
            using var readContext = _fixture.CreateNewContext();
            var readService = new OrganizationService(readContext);
            var result = await readService.GetOrganizationByIdAsync(org.OrganizationId);

            Assert.NotNull(result);
            Assert.Equal("Test Organization", result.Name);
            Assert.Equal("test@org.com", result.ContactEmail);
            Assert.True(result.IsVerified);
        }

        [Fact]
        public async Task AddMultiple_And_GetAll_ReturnsAll()
        {
            await _service.AddOrganizationAsync(new Organization { Name = "Org A", ContactEmail = "a@org.com" });
            await _service.AddOrganizationAsync(new Organization { Name = "Org B", ContactEmail = "b@org.com" });
            await _service.AddOrganizationAsync(new Organization { Name = "Org C", ContactEmail = "c@org.com" });

            using var readContext = _fixture.CreateNewContext();
            var readService = new OrganizationService(readContext);
            var result = await readService.GetAllOrganizationsAsync();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task UpdateOrganization_ModifiesEntity()
        {
            var org = new Organization { Name = "Original", ContactEmail = "orig@org.com" };
            await _service.AddOrganizationAsync(org);

            org.Name = "Updated Name";
            org.IsVerified = true;
            await _service.UpdateOrganizationAsync(org);

            using var readContext = _fixture.CreateNewContext();
            var readService = new OrganizationService(readContext);
            var result = await readService.GetOrganizationByIdAsync(org.OrganizationId);

            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.True(result.IsVerified);
        }

        [Fact]
        public async Task DeleteOrganization_RemovesEntity()
        {
            var org = new Organization { Name = "To Delete", ContactEmail = "del@org.com" };
            await _service.AddOrganizationAsync(org);

            await _service.DeleteOrganizationAsync(org.OrganizationId);

            using var readContext = _fixture.CreateNewContext();
            var readService = new OrganizationService(readContext);
            var result = await readService.GetOrganizationByIdAsync(org.OrganizationId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPaginated_ReturnsCorrectPage()
        {
            for (int i = 0; i < 15; i++)
            {
                await _service.AddOrganizationAsync(new Organization
                {
                    Name = $"Org {i:D2}",
                    ContactEmail = $"org{i}@test.com"
                });
            }

            using var readContext = _fixture.CreateNewContext();
            var readService = new OrganizationService(readContext);

            var page1 = await readService.GetPaginatedOrganizationsAsync(1, 10);
            Assert.Equal(10, page1.Count);
            Assert.Equal(15, page1.TotalCount);
            Assert.True(page1.HasNextPage);
            Assert.False(page1.HasPreviousPage);

            var page2 = await readService.GetPaginatedOrganizationsAsync(2, 10);
            Assert.Equal(5, page2.Count);
            Assert.False(page2.HasNextPage);
            Assert.True(page2.HasPreviousPage);
        }

        [Fact]
        public void OrganizationExists_ReturnsTrueForExisting()
        {
            var org = new Organization { Name = "Exists", ContactEmail = "e@org.com" };
            _fixture.Context.Organizations.Add(org);
            _fixture.Context.SaveChanges();

            Assert.True(_service.OrganizationExists(org.OrganizationId));
            Assert.False(_service.OrganizationExists(Guid.NewGuid()));
        }

        public void Dispose() => _fixture.Dispose();
    }
}
