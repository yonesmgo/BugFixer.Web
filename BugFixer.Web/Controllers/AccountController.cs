using BugFixer.Application.Securities;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Acount;
using BugFixer.Web.ActionFilters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static BugFixer.Domain.ViewModels.Acount.ForgotPasswordViewModel;

namespace BugFixer.Web.Controllers
{
    public class AccountController : BaseController
    {
        #region ctor
        private readonly IUserServices _userServices;
        public AccountController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        #endregion
        #region Login
        [HttpGet("Login")]
        [RedirectHomeLogedInActionFilter]
        public IActionResult Login(string ReturnUrl = "")
        {
            var res = new LoginViewModel();
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                res.ReturnUrl = ReturnUrl;
            }
            return View(res);
        }
        [HttpPost("Login"), ValidateAntiForgeryToken]
        [RedirectHomeLogedInActionFilter]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            var res = await _userServices.CheckUserIsLogin(login);
            switch (res)
            {
                case LoginResult.success:

                    var user = await _userServices.GetUSerByEMail(login.Email.ToLower().Trim().SanitizeText());
                    #region loginuser
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principle = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = login.RemmberMe
                    };
                    await HttpContext.SignInAsync(principle, properties);
                    #endregion
                    TempData[successMessage] = "خوش آمدید";
                    if (!string.IsNullOrEmpty(login.ReturnUrl))
                    {
                        return Redirect(login.ReturnUrl);
                    }
                    return Redirect("/");
                case LoginResult.UserIsBan:
                    TempData[WarningMessage] = "دسترسی شما به سایت مسدود میباشد";
                    break;
                case LoginResult.UserNotFound:
                    TempData[errorMessage] = "کاربر مورد نظر یافت نشد";
                    break;
                case LoginResult.EmailNotActivated:
                    TempData[WarningMessage] = "برای ورود به حساب کاربری ایمیل خود را فعال فرمایید";
                    break;
            }
            return View(login);
        }
        #endregion
        #region Register
        [HttpGet("Register")]
        [RedirectHomeLogedInActionFilter]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("Register"), ValidateAntiForgeryToken]
        [RedirectHomeLogedInActionFilter]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            var result = await _userServices.RegisterUser(register);
            switch (result)
            {
                case RegisterResult.EmailExists:
                    TempData[errorMessage] = "ایمیل وارد شده از قبل موجود است ";
                    break;
                case RegisterResult.success:
                    TempData[successMessage] = "عملیات با موفقیت انجام شد ";
                    return RedirectToAction("Register", "Account");
            }
            return View(register);
        }
        #endregion
        #region Logout
        [HttpGet("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        #endregion
        #region Email Activation
        [HttpGet("Activate-Email/{ActivationCode}")]
        [RedirectHomeLogedInActionFilter]
        public async Task<IActionResult> ActivationUserEmai(string ActivationCode)
        {
            var res = await _userServices.ActivateUserEmail(ActivationCode);
            if (res)
            {
                TempData[successMessage] = "حساب کاربری شما با موفقیت فعال شد ";
            }
            else
            {
                TempData[errorMessage] = "فعال سازی حساب کاربری با خطا مواجه شد ";
            }
            return RedirectToAction("Login", "Account");
        }
        #endregion
        #region Forgot Password
        [HttpGet("Forgot-Password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost("Forgot-Password"), ValidateAntiForgeryToken]
        [RedirectHomeLogedInActionFilter]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgot)
        {
            var res = await _userServices.ForgotPassword(forgot);
            switch (res)
            {
                case ForgotPasswordResult.UserBan:
                    TempData[WarningMessage] = "حساب کاربری شما مسدود میباشد ";
                    break;
                case ForgotPasswordResult.UserNotFind:
                    TempData[WarningMessage] = "حسابی با این عنوان پیدا نشد";
                    break;
                case ForgotPasswordResult.Success:
                    TempData[successMessage] = "ایمیل اعتبار سنجی برای شما ارسال شد";
                    break;
                default:
                    break;
            }
            return View(forgot);
        }
        #endregion

        #region Reset Password
        [HttpGet("Reset-Password/{EmailActivationCode}")]
        public async Task<IActionResult> ResetPassword(string EmailActivationCode)
        {
            var user = await _userServices.GetUSerByActivationCode(EmailActivationCode);
            if (user == null || user.IsDelete || user.IsBan)
                return NotFound();

            return View(new RessetPasswordViewModel
            {
                EmailActivationCode = user.EmailActivationCode,
            });
        }
        [HttpPost("Reset-Password/{EmailActivationCode}"), ValidateAntiForgeryToken]
        [RedirectHomeLogedInActionFilter]
        public async Task<IActionResult> ResetPassword(RessetPasswordViewModel resset)
        {
            if (!ModelState.IsValid)
            {
                return View(resset);
            }
            RessetPasswordReuslt res = await _userServices.RestPassword(resset);
            switch (res)
            {
                case RessetPasswordReuslt.Success:
                    TempData[successMessage] = "رمز شما با موفقیت تغییر کرد";
                    return RedirectToAction("Login", "Account");
                case RessetPasswordReuslt.UserNotFind:
                    TempData[WarningMessage] = "حساب کاربری با این عنوان پیدا نشد ";
                    return View(resset);
                case RessetPasswordReuslt.userisban:
                    return View(resset);
            }
            return View(resset);
        }
        #endregion

    }
}
