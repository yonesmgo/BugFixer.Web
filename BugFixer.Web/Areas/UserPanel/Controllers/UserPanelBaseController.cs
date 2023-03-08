using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class UserPanelBaseController : Controller
    {
        public static string successMessage = "SuccessMessage";
        public static string WarningMessage = "WarningMessage";
        public static string infoMessage = "infoMessage";
        public static string errorMessage = "errorMessage";
    }
}
