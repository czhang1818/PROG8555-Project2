using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VSMS.Web2.Models
{
    public class Opportunity
    {
        [Key]
        public Guid OpportunityId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; } = DateTime.Now;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Range(1, 1000)]
        [Display(Name = "Max Volunteers")]
        public int MaxVolunteers { get; set; }

        // Navigation Properties
        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }
        public ICollection<Application>? Applications { get; set; }
        public ICollection<OpportunitySkill>? OpportunitySkills { get; set; }
    }
}
