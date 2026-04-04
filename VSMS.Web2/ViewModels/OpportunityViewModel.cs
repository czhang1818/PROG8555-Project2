using Microsoft.AspNetCore.Mvc.Rendering;
using VSMS.Web2.Models;

namespace VSMS.Web2.ViewModels
{
    public class OpportunityViewModel
    {
        public Opportunity Opportunity { get; set; } = new Opportunity();
        public IEnumerable<SelectListItem>? Organizations { get; set; }
    }
}
