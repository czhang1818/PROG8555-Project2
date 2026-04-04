using System.ComponentModel.DataAnnotations;
using VSMS.Web2.Models;

namespace VSMS.Web2.ViewModels
{
    public class OpportunitySkillViewModel
    {
        public Opportunity Opportunity { get; set; } = new Opportunity();

        [Display(Name = "Required Skills")]
        public List<Guid> SelectedSkillIds { get; set; } = new List<Guid>();

        public List<Skill> AvailableSkills { get; set; } = new List<Skill>();
    }
}
