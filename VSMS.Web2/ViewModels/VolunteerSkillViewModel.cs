using System.ComponentModel.DataAnnotations;
using VSMS.Web2.Models;

namespace VSMS.Web2.ViewModels
{
    public class VolunteerSkillViewModel
    {
        public Volunteer Volunteer { get; set; } = new Volunteer();

        [Display(Name = "Volunteer Skills")]
        public List<Guid> SelectedSkillIds { get; set; } = new List<Guid>();

        public List<Skill> AvailableSkills { get; set; } = new List<Skill>();
    }
}
