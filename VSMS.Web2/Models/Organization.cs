using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VSMS.Web2.Models
{
    public class Organization
    {
        [Key]
        public Guid OrganizationId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Url]
        public string? Website { get; set; }

        [Display(Name = "Is Verified")]
        public bool IsVerified { get; set; } = false;

        // Navigation Properties
        public ICollection<Opportunity>? Opportunities { get; set; }
        public ICollection<Coordinator>? Coordinators { get; set; }
    }
}
