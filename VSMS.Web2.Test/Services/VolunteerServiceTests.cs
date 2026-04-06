using VSMS.Web2.Models;
using VSMS.Web2.Services;
using VSMS.Web2.Test.Fixtures;

namespace VSMS.Web2.Test.Services
{
    public class VolunteerServiceTests : IDisposable
    {
        private readonly DatabaseFixture _fixture;
        private readonly VolunteerService _service;

        public VolunteerServiceTests()
        {
            _fixture = new DatabaseFixture();
            _service = new VolunteerService(_fixture.Context);
        }

        [Fact]
        public async Task GetAllVolunteers_ReturnsEmpty_Initially()
        {
            var result = await _service.GetAllVolunteersAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddVolunteer_And_GetById_ReturnsVolunteer()
        {
            var vol = new Volunteer
            {
                Name = "Alice Johnson",
                Email = "alice@test.com",
                PhoneNumber = "555-0101",
                TotalHours = 25.5f
            };

            await _service.AddVolunteerAsync(vol);

            using var readContext = _fixture.CreateNewContext();
            var readService = new VolunteerService(readContext);
            var result = await readService.GetVolunteerByIdAsync(vol.VolunteerId);

            Assert.NotNull(result);
            Assert.Equal("Alice Johnson", result.Name);
            Assert.Equal("alice@test.com", result.Email);
            Assert.Equal(25.5f, result.TotalHours);
        }

        [Fact]
        public async Task AddMultiple_And_GetAll_ReturnsAll()
        {
            await _service.AddVolunteerAsync(new Volunteer { Name = "V1", Email = "v1@t.com", PhoneNumber = "111" });
            await _service.AddVolunteerAsync(new Volunteer { Name = "V2", Email = "v2@t.com", PhoneNumber = "222" });

            using var readContext = _fixture.CreateNewContext();
            var readService = new VolunteerService(readContext);
            var result = await readService.GetAllVolunteersAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateVolunteer_ModifiesEntity()
        {
            var vol = new Volunteer { Name = "Before", Email = "b@t.com", PhoneNumber = "000" };
            await _service.AddVolunteerAsync(vol);

            vol.Name = "After";
            vol.TotalHours = 100f;
            await _service.UpdateVolunteerAsync(vol);

            using var readContext = _fixture.CreateNewContext();
            var readService = new VolunteerService(readContext);
            var result = await readService.GetVolunteerByIdAsync(vol.VolunteerId);

            Assert.NotNull(result);
            Assert.Equal("After", result.Name);
            Assert.Equal(100f, result.TotalHours);
        }

        [Fact]
        public async Task DeleteVolunteer_RemovesEntity()
        {
            var vol = new Volunteer { Name = "Del", Email = "d@t.com", PhoneNumber = "000" };
            await _service.AddVolunteerAsync(vol);

            await _service.DeleteVolunteerAsync(vol.VolunteerId);

            using var readContext = _fixture.CreateNewContext();
            var readService = new VolunteerService(readContext);
            var result = await readService.GetVolunteerByIdAsync(vol.VolunteerId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPaginated_ReturnsCorrectPage()
        {
            for (int i = 0; i < 8; i++)
            {
                await _service.AddVolunteerAsync(new Volunteer
                {
                    Name = $"Vol {i:D2}",
                    Email = $"vol{i}@t.com",
                    PhoneNumber = $"555-{i:D4}"
                });
            }

            using var readContext = _fixture.CreateNewContext();
            var readService = new VolunteerService(readContext);

            var page1 = await readService.GetPaginatedVolunteersAsync(1, 5);
            Assert.Equal(5, page1.Count);
            Assert.Equal(8, page1.TotalCount);
            Assert.True(page1.HasNextPage);

            var page2 = await readService.GetPaginatedVolunteersAsync(2, 5);
            Assert.Equal(3, page2.Count);
            Assert.False(page2.HasNextPage);
        }

        [Fact]
        public async Task AddOrUpdateVolunteerSkills_AssignsSkills()
        {
            // Create volunteer and skills
            var vol = new Volunteer { Name = "Skilled Vol", Email = "sv@t.com", PhoneNumber = "000" };
            await _service.AddVolunteerAsync(vol);

            var skill1 = new Skill { Name = "Cooking", Category = "Culinary" };
            var skill2 = new Skill { Name = "Driving", Category = "Transport" };
            _fixture.Context.Skills.AddRange(skill1, skill2);
            await _fixture.Context.SaveChangesAsync();

            // Assign skills
            await _service.AddOrUpdateVolunteerSkillsAsync(vol.VolunteerId, new List<Guid> { skill1.SkillId, skill2.SkillId });

            using var readContext = _fixture.CreateNewContext();
            var readService = new VolunteerService(readContext);
            var result = await readService.GetVolunteerByIdAsync(vol.VolunteerId);

            Assert.NotNull(result);
            Assert.NotNull(result.VolunteerSkills);
            Assert.Equal(2, result.VolunteerSkills.Count);
        }

        [Fact]
        public async Task AddOrUpdateVolunteerSkills_ReplacesExistingSkills()
        {
            var vol = new Volunteer { Name = "Replace Skills", Email = "rs@t.com", PhoneNumber = "000" };
            await _service.AddVolunteerAsync(vol);

            var skill1 = new Skill { Name = "A", Category = "X" };
            var skill2 = new Skill { Name = "B", Category = "X" };
            var skill3 = new Skill { Name = "C", Category = "X" };
            _fixture.Context.Skills.AddRange(skill1, skill2, skill3);
            await _fixture.Context.SaveChangesAsync();

            // Assign initial skills
            await _service.AddOrUpdateVolunteerSkillsAsync(vol.VolunteerId, new List<Guid> { skill1.SkillId, skill2.SkillId });

            // Replace with new skills
            await _service.AddOrUpdateVolunteerSkillsAsync(vol.VolunteerId, new List<Guid> { skill3.SkillId });

            using var readContext = _fixture.CreateNewContext();
            var readService = new VolunteerService(readContext);
            var result = await readService.GetVolunteerByIdAsync(vol.VolunteerId);

            Assert.NotNull(result);
            Assert.NotNull(result.VolunteerSkills);
            Assert.Single(result.VolunteerSkills);
            Assert.Equal(skill3.SkillId, result.VolunteerSkills.First().SkillId);
        }

        [Fact]
        public void VolunteerExists_ReturnsTrueForExisting()
        {
            var vol = new Volunteer { Name = "Exists", Email = "e@t.com", PhoneNumber = "000" };
            _fixture.Context.Volunteers.Add(vol);
            _fixture.Context.SaveChanges();

            Assert.True(_service.VolunteerExists(vol.VolunteerId));
            Assert.False(_service.VolunteerExists(Guid.NewGuid()));
        }

        public void Dispose() => _fixture.Dispose();
    }
}
