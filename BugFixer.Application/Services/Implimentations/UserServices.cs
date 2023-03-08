using BugFixer.Application.Extentions;
using BugFixer.Application.Generators;
using BugFixer.Application.Securities;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Enums;
using BugFixer.Domain.Interface;
using BugFixer.Domain.ViewModels.Acount;
using BugFixer.Domain.ViewModels.UserPanel.Account;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System.Data;
using static BugFixer.Domain.ViewModels.Acount.ForgotPasswordViewModel;

namespace BugFixer.Application.Services.Implimentations
{
    public class UserServices : IUserServices
    {
        #region ctor
        private readonly IUserRepository _userRepository;
        private readonly IEmailServices _EmailServices;
        private readonly ScoreManagementViewModel _scoreManagement;
        public UserServices(IUserRepository userRepository, IEmailServices emailServices, IOptions<ScoreManagementViewModel> scoreManagement)
        {
            _userRepository = userRepository;
            _EmailServices = emailServices;
            _scoreManagement = scoreManagement.Value;
        }
        #endregion
        #region Register
        public async Task<RegisterResult> RegisterUser(RegisterViewModel model)
        {
            //check email exists
            if (await _userRepository.isExistUserByEmail(model.Email.Trim().ToLower()))
            {
                return RegisterResult.EmailExists;
            }

            //hash password 
            var password = PaswordHelper.EncodePasswordMD5(model.Password.SanitizeText());

            //Create User
            var user = new User
            {
                Avatar = PathTools.DefaultUserAvatar,
                Email = model.Email.SanitizeText().Trim().ToLower(),
                Password = password,
                EmailActivationCode = CodeGenerator.CreateActivationCode()
            };

            //add to data base 
            await _userRepository.CreateUSer(user);
            await _userRepository.save();

            //Send Activation Emai
            #region Send Activation Email

            var body = $@"<div>برای فعالسازی حسلب کاربری خود روی لینک زیر کلیک کنید</div>
                          <a href='{PathTools.SiteAddress}/Activate-Email/{user.EmailActivationCode}'>فعالسازی حساب کاربری </a>";
            _EmailServices.SendEmail(user.Email, "فعالسازی حساب کاربری ", body);
            #endregion

            return RegisterResult.success;

        }
        #endregion
        #region Login
        public async Task<LoginResult> CheckUserIsLogin(LoginViewModel login)
        {
            var user = await _userRepository.GetuserByEMail(login.Email.Trim().ToLower().SanitizeText());
            if (user == null)
            {
                return LoginResult.UserNotFound;
            }
            var hashedpassword = PaswordHelper.EncodePasswordMD5(login.Password.SanitizeText());
            if (hashedpassword != user.Password)
            {
                return LoginResult.UserNotFound;
            }
            if (user.IsDelete)
            {
                return LoginResult.UserNotFound;

            }
            if (user.IsBan)
            {
                return LoginResult.UserIsBan;
            }
            if (!user.IsEmailConfirmed)
            {
                return LoginResult.EmailNotActivated;
            }
            return LoginResult.success;
        }
        public async Task<User> GetUSerByEMail(string v)
        {
            return await _userRepository.GetuserByEMail(v.Trim().ToLower().SanitizeText());
        }


        #endregion
        #region Activvation Email
        public async Task<bool> ActivateUserEmail(string activationCode)
        {
            var user = await _userRepository.GetUserByActivationCode(activationCode);
            if (user == null) return false;
            if (user.IsBan || user.IsDelete) return false;
            user.IsEmailConfirmed = true;
            user.EmailActivationCode = CodeGenerator.CreateActivationCode();
            await _userRepository.Update(user);
            await _userRepository.save();
            return true;
        }

        #endregion
        #region Forgot Password

        public async Task<ForgotPasswordResult> ForgotPassword(ForgotPasswordViewModel forgot)
        {
            var email = forgot.Email.SanitizeText().Trim().ToLower();
            var user = await _userRepository.GetuserByEMail(email);
            if (user == null || user.IsDelete)
            {
                return ForgotPasswordResult.UserNotFind;
            }
            if (user.IsBan)
            {
                return ForgotPasswordResult.UserBan;
            }
            #region Send Forgot Email

            var body = $@"<div>برای فعالسازی حسلب کاربری خود روی لینک زیر کلیک کنید</div>
                          <a href='{PathTools.SiteAddress}/Activate-Email/{user.EmailActivationCode}'>فراموشی کلمه عبور  </a>";
            _EmailServices.SendEmail(user.Email, "فعالسازی حساب کاربری ", body);
            #endregion
            return ForgotPasswordResult.Success;


        }


        #endregion


        #region Reset Password
        public async Task<RessetPasswordReuslt> RestPassword(RessetPasswordViewModel resset)
        {
            var user = await _userRepository.GetUserByActivationCode(resset.EmailActivationCode.SanitizeText().Trim().ToLower());
            if (user == null || user.IsDelete) return RessetPasswordReuslt.UserNotFind;
            if (user.IsBan) return RessetPasswordReuslt.userisban;
            var pass = PaswordHelper.EncodePasswordMD5(resset.Password.SanitizeText());

            user.Password = pass;
            user.IsEmailConfirmed = true;
            user.EmailActivationCode = CodeGenerator.CreateActivationCode();
            await _userRepository.Update(user);
            await _userRepository.save();
            return RessetPasswordReuslt.Success;


        }

        public async Task<User> GetUSerByActivationCode(string emailActivationCode)
        {
            return await _userRepository.GetUserByActivationCode(emailActivationCode.SanitizeText());
        }

        public async Task<User> GetUserByID(long userid)
        {
            var user = await _userRepository.getUserbyID(userid);
            return user;
        }

        public async Task ChangeUserAvatar(long userID, string Filename)
        {
            var user = await GetUserByID(userID);
            user.Avatar = Filename;
            await _userRepository.Update(user);
            await _userRepository.save();
        }




        #endregion


        public async Task<EditUserViewModel> FillEditUserViewModel(long ID)
        {
            var res = await _userRepository.getUserbyID(ID);
            var edituser = new EditUserViewModel()
            {
                BirthDate = res.BirthDate != null ? DateExtensions.ToShamsi(res.BirthDate.Value).ToString() : string.Empty,
                CityId = res.CityId,
                CountryId = res.CountryId,
                Description = res.Description,
                FirstName = res.FirstName ?? string.Empty,
                LastName = res.LastName ?? string.Empty,
                GetNewsLetter = res.GetNewsLetter,

                PhoneNumber = res.PhoneNumber
            };
            return edituser;

        }

        public async Task<EditUserResult> EditUserInfod(EditUserViewModel editUser, long v)
        {
            var user = await _userRepository.getUserbyID(v);
            if (!string.IsNullOrEmpty(editUser.BirthDate))
            {
                try
                {
                    var date = editUser.BirthDate.ToMiladi();
                    user.BirthDate = date;
                }
                catch (Exception)
                {
                    return EditUserResult.NotValidate;
                }
            }
            user.FirstName = editUser.FirstName;
            user.LastName = editUser.LastName;
            user.PhoneNumber = editUser.PhoneNumber;
            user.Description = editUser.Description;
            user.GetNewsLetter = editUser.GetNewsLetter;
            user.CityId = editUser.CityId;
            user.CountryId = editUser.CountryId;
            await _userRepository.Update(user);
            await _userRepository.save();
            return EditUserResult.Success;
        }

        #region User Score
        public async Task UpdateUSerScoreAndMedal(long UserID, long Score)
        {
            var user = await GetUserByID(UserID);
            if (user == null) return;
            user.Score += int.Parse(Score.ToString());
            await _userRepository.Update(user);
            await _userRepository.save();
            if (user.Score >= _scoreManagement.MinsScoreForBronzeMedal && user.Score < _scoreManagement.MinsScoreForSilverMedal)
            {
                if (user.Medal is not null && user.Medal == UserMedal.Bronze)
                {
                    return;
                }
                user.Medal = UserMedal.Bronze;
                await _userRepository.Update(user);
                await _userRepository.save();
            }

            else if (user.Score >= _scoreManagement.MinsScoreForSilverMedal && user.Score < _scoreManagement.MinsScoreForGoldMedal)
            {
                if (user.Medal is not null && user.Medal == UserMedal.Silver)
                {
                    return;
                }
                user.Medal = UserMedal.Silver;
                await _userRepository.Update(user);
                await _userRepository.save();
            }
            else if (user.Score >= _scoreManagement.MinsScoreForGoldMedal)
            {
                if (user.Medal is not null && user.Medal == UserMedal.Silver)
                {
                    return;
                }
                user.Medal = UserMedal.Gold;
                await _userRepository.Update(user);
                await _userRepository.save();
            }
        }
        #endregion

    }

}
