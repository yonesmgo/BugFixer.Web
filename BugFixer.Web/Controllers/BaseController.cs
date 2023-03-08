using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Controllers
{
    public class BaseController : Controller
    {
        public static string successMessage = "SuccessMessage";
        public static string WarningMessage = "WarningMessage";
        public static string infoMessage = "infoMessage";
        public static string errorMessage = "errorMessage";
    }
}
