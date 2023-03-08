using BugFixer.Application.Extentions;
using BugFixer.Application.Services.Implimentations;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.UserPanel.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    public class AccountController : UserPanelBaseController
    {
        private readonly IStateServices _state;
        private readonly IUserServices _services;
        #region Ctore
        public AccountController(IStateServices state, IUserServices services)
        {
            _state = state;
            _services = services;
        }
        #endregion
        #region Edit User info
        public async Task<IActionResult> EditInfo()
        {
            ViewData["State"] = await _state.GetAllState();
            var result = await _services.FillEditUserViewModel(HttpContext.User.GetUserId());
            if (result.CountryId.HasValue)
            {
                ViewData["Cities"] = await _state.GetAllState(result.CountryId);
            }
            return View(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInfo(EditUserViewModel editUser)
        {
            var result = await _services.EditUserInfod(editUser, HttpContext.User.GetUserId());
            if (ModelState.IsValid)
            {
                switch (result)
                {
                    case EditUserResult.Success:
                        TempData[successMessage] = "تغییر مشخصات با موفقیتانجام شد ";
                        return RedirectToAction("EditInfo", "Account", new { area = "UserPanel" });
                    case EditUserResult.NotValidate:
                        TempData[errorMessage] = "تاریخ وارد شده معتبر نمیباشد ";
                        break;
                }
            }
            if (editUser.CountryId.HasValue)
            {
                ViewData["Cities"] = await _state.GetAllState(editUser.CountryId);
            }
            ViewData["State"] = await _state.GetAllState();
            return View(editUser);
        }
        #endregion
        public async Task<IActionResult> LoadCities(long CountryId)
        {
            var res = await _state.GetAllState(CountryId);
            return new JsonResult(res);
        }
    }
}
