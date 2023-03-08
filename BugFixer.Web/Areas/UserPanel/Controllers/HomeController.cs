using BugFixer.Application.Extentions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    public class HomeController : UserPanelBaseController
    {
        private IUserServices _services;
        #region CTOR
        public HomeController(IUserServices services)
        {
            _services = services;
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }
        #region ChangeUserAvatar
        public async Task<IActionResult> ChangeUserAvatar(IFormFile formFile)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
            var validformat = new List<string>()
            {
                ".png",
                ".jpg",
                ".jpeg"
            };
            var res = formFile.UploadFile(filename, PathTools.USerAvatarServerPath, validformat);
            if (!res)
            {
                return new JsonResult(new
                {
                    status = "Error"
                });
            }
            await _services.ChangeUserAvatar(HttpContext.User.GetUserId(), filename);
            TempData[successMessage] = "عملیات با موفقیت انجام شد ";
            return new JsonResult(new
            {
                status = "Success"
            });
        }
        #endregion

    }
}
