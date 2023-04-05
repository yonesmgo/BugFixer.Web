using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
