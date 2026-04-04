using Microsoft.AspNetCore.Mvc.Rendering;
using VSMS.Web2.Models;

namespace VSMS.Web2.ViewModels
{
    public class ApplicationViewModel
    {
        public Application Application { get; set; } = new Application();
        public IEnumerable<SelectListItem>? Volunteers { get; set; }
        public IEnumerable<SelectListItem>? Opportunities { get; set; }
    }
}
