using BugFixer.Application.Extentions;
using BugFixer.Application.Services.Implimentations;
using BugFixer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.ViewComponents
{
    public class UserSidebarViewComponent : ViewComponent
    {
        private IUserServices _userServices;
        #region Ctor
        public UserSidebarViewComponent(IUserServices userServices)
        {
            _userServices = userServices;
        }
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userServices.GetUserByID(HttpContext.User.GetUserId());
            //HttpContext.User.GetUserId()
            return View("UserSidebar", user);
        }

    }
}
