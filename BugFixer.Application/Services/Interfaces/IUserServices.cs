using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.ViewModels.Acount;
using BugFixer.Domain.ViewModels.UserPanel.Account;
using static BugFixer.Domain.ViewModels.Acount.ForgotPasswordViewModel;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IUserServices
    {
        Task<RegisterResult> RegisterUser(RegisterViewModel model);
        Task<LoginResult> CheckUserIsLogin(LoginViewModel login);
        Task<User> GetUSerByEMail(string v);
        #region Email Activation
        Task<bool> ActivateUserEmail(string activationCode);
        #endregion

        #region Forgot Password
        Task<ForgotPasswordResult> ForgotPassword(ForgotPasswordViewModel forgot);
        Task<RessetPasswordReuslt> RestPassword(RessetPasswordViewModel resset);
        Task<User> GetUSerByActivationCode(string emailActivationCode);
        Task<User> GetUserByID(long userid);
        #endregion

        Task ChangeUserAvatar(long userID, string Filename);

        Task<EditUserViewModel> FillEditUserViewModel(long ID);
        Task<EditUserResult> EditUserInfod(EditUserViewModel editUser, long v);

        #region User Question 
        Task UpdateUSerScoreAndMedal(long UserID, long Score);
        #endregion
    }
}
