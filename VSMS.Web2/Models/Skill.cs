using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VSMS.Web2.Models
{
    public class Skill
    {
        [Key]
        public Guid SkillId { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Display(Name = "Requires Certification")]
        public bool RequiresCertification { get; set; }

        // Navigation Properties
        public ICollection<VolunteerSkill>? VolunteerSkills { get; set; }
        public ICollection<OpportunitySkill>? OpportunitySkills { get; set; }
    }
}
