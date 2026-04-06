using VSMS.Web2.Models;
using VSMS.Web2.Services;
using VSMS.Web2.Test.Fixtures;

namespace VSMS.Web2.Test.Services
{
    public class SkillServiceTests : IDisposable
    {
        private readonly DatabaseFixture _fixture;
        private readonly SkillService _service;

        public SkillServiceTests()
        {
            _fixture = new DatabaseFixture();
            _service = new SkillService(_fixture.Context);
        }

        [Fact]
        public async Task GetAllSkills_ReturnsEmpty_Initially()
        {
            var result = await _service.GetAllSkillsAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddSkill_And_GetById_ReturnsSkill()
        {
            var skill = new Skill
            {
                Name = "First Aid",
                Category = "Medical",
                Description = "Basic first aid and CPR",
                RequiresCertification = true
            };

            await _service.AddSkillAsync(skill);

            using var readContext = _fixture.CreateNewContext();
            var readService = new SkillService(readContext);
            var result = await readService.GetSkillByIdAsync(skill.SkillId);

            Assert.NotNull(result);
            Assert.Equal("First Aid", result.Name);
            Assert.Equal("Medical", result.Category);
            Assert.True(result.RequiresCertification);
        }

        [Fact]
        public async Task AddMultiple_And_GetAll_ReturnsAll()
        {
            await _service.AddSkillAsync(new Skill { Name = "Cooking", Category = "Culinary" });
            await _service.AddSkillAsync(new Skill { Name = "Driving", Category = "Transport" });
            await _service.AddSkillAsync(new Skill { Name = "Tutoring", Category = "Education" });

            using var readContext = _fixture.CreateNewContext();
            var readService = new SkillService(readContext);
            var result = await readService.GetAllSkillsAsync();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task UpdateSkill_ModifiesEntity()
        {
            var skill = new Skill { Name = "Original Skill", Category = "General" };
            await _service.AddSkillAsync(skill);

            skill.Name = "Updated Skill";
            skill.RequiresCertification = true;
            await _service.UpdateSkillAsync(skill);

            using var readContext = _fixture.CreateNewContext();
            var readService = new SkillService(readContext);
            var result = await readService.GetSkillByIdAsync(skill.SkillId);

            Assert.NotNull(result);
            Assert.Equal("Updated Skill", result.Name);
            Assert.True(result.RequiresCertification);
        }

        [Fact]
        public async Task DeleteSkill_RemovesEntity()
        {
            var skill = new Skill { Name = "To Delete", Category = "Temp" };
            await _service.AddSkillAsync(skill);

            await _service.DeleteSkillAsync(skill.SkillId);

            using var readContext = _fixture.CreateNewContext();
            var readService = new SkillService(readContext);
            var result = await readService.GetSkillByIdAsync(skill.SkillId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPaginated_ReturnsCorrectPage()
        {
            for (int i = 0; i < 12; i++)
            {
                await _service.AddSkillAsync(new Skill
                {
                    Name = $"Skill {i:D2}",
                    Category = "Test"
                });
            }

            using var readContext = _fixture.CreateNewContext();
            var readService = new SkillService(readContext);

            var page1 = await readService.GetPaginatedSkillsAsync(1, 5);
            Assert.Equal(5, page1.Count);
            Assert.Equal(12, page1.TotalCount);
            Assert.True(page1.HasNextPage);

            var page3 = await readService.GetPaginatedSkillsAsync(3, 5);
            Assert.Equal(2, page3.Count);
            Assert.False(page3.HasNextPage);
        }

        [Fact]
        public void SkillExists_ReturnsTrueForExisting()
        {
            var skill = new Skill { Name = "Exists", Category = "Test" };
            _fixture.Context.Skills.Add(skill);
            _fixture.Context.SaveChanges();

            Assert.True(_service.SkillExists(skill.SkillId));
            Assert.False(_service.SkillExists(Guid.NewGuid()));
        }

        public void Dispose() => _fixture.Dispose();
    }
}
