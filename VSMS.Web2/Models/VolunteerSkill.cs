using System;

namespace VSMS.Web2.Models
{
    public class VolunteerSkill
    {
        public Guid VolunteerId { get; set; } // Composite PK / FK
        public Guid SkillId { get; set; }     // Composite PK / FK

        public string ProficiencyLevel { get; set; } = string.Empty;
        public DateTime AcquiredDate { get; set; }

        public Volunteer? Volunteer { get; set; }
        public Skill? Skill { get; set; }
    }
}
