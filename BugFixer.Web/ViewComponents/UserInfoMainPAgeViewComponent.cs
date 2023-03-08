using BugFixer.Application.Extentions;
using BugFixer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.ViewComponents
{
    public class UserInfoMainPAgeViewComponent : ViewComponent
    {
        private IUserServices _userServices;
        #region Ctor
        public UserInfoMainPAgeViewComponent(IUserServices userServices)
        {
            _userServices = userServices;
        }
        #endregion
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userServices.GetUserByID(HttpContext.User.GetUserId());
            //HttpContext.User.GetUserId()
            return View("UserInfoMainPAge", user);
        }
    }
}
