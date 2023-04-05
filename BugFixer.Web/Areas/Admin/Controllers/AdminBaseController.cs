using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminBaseController : Controller
    {
        public static string successMessage = "SuccessMessage";
        public static string WarningMessage = "WarningMessage";
        public static string infoMessage = "infoMessage";
        public static string errorMessage = "errorMessage";
    }
}
