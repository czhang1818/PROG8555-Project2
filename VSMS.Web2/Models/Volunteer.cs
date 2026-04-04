using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VSMS.Web2.Models
{
    public class Volunteer
    {
        [Key]
        public Guid VolunteerId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Total Hours")]
        public float TotalHours { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK to ApplicationUser (1-to-1)
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        // Navigation Properties
        public ICollection<Application>? Applications { get; set; }
        public ICollection<VolunteerSkill>? VolunteerSkills { get; set; }
    }
}
