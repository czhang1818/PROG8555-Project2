using System;

namespace VSMS.Web2.Models
{
    public class OpportunitySkill
    {
        public Guid OpportunityId { get; set; } // Composite PK / FK
        public Guid SkillId { get; set; }       // Composite PK / FK

        public bool IsMandatory { get; set; }
        public string MinimumLevel { get; set; } = string.Empty;

        public Opportunity? Opportunity { get; set; }
        public Skill? Skill { get; set; }
    }
}
