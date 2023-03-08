using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Interface;
using System.Net.Mail;

namespace BugFixer.Application.Services.Implimentations
{
    public class EmailServices : IEmailServices
    {
        #region Ctor
        private readonly ISettingSiteReporistory _setting;

        public EmailServices(ISettingSiteReporistory setting)
        {
            _setting = setting;
        }
        #endregion
        #region Email Send
        public async Task<bool> SendEmail(string to, string subject, string body)
        {
            try
            {
                var defaultSiteEmail = await _setting.GetDefaultEmail();
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient(defaultSiteEmail.SMTP);
                mail.From = new MailAddress(defaultSiteEmail.From, defaultSiteEmail.DisplayName);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml= true;
                if (defaultSiteEmail.Port != 0)
                {
                    smtpClient.Port = defaultSiteEmail.Port;
                    smtpClient.EnableSsl= true;
                }
                smtpClient.Credentials = new System.Net.NetworkCredential(defaultSiteEmail.From, defaultSiteEmail.Password);
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion



    }
}
