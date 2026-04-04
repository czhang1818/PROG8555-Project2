using Microsoft.AspNetCore.Mvc.Rendering;
using VSMS.Web2.Models;

namespace VSMS.Web2.ViewModels
{
    public class CoordinatorViewModel
    {
        public Coordinator Coordinator { get; set; } = new Coordinator();
        public IEnumerable<SelectListItem>? Organizations { get; set; }
    }
}
