using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VSMS.Web2.Models
{
    public class Application
    {
        [Key]
        public Guid AppId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid VolunteerId { get; set; }

        [Required]
        public Guid OpportunityId { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        // Audit tracking fields (Part 2 requirement)
        [Display(Name = "Created By")]
        public string? CreatedByUserId { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified At")]
        public DateTime? LastModifiedAt { get; set; }

        // Navigation Properties
        [ForeignKey("VolunteerId")]
        public Volunteer? Volunteer { get; set; }

        [ForeignKey("OpportunityId")]
        public Opportunity? Opportunity { get; set; }
    }
}
