using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.ViewModels.Acount
{
    public class RessetPasswordViewModel
    {
        [Required]
        public string EmailActivationCode { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }
        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Compare("Password", ErrorMessage = "کلمه عبور مغایرت دارد ")]
        public string RePassword { get; set; }
    }
    public enum RessetPasswordReuslt
    {
        Success,
        UserNotFind,
        userisban
    }
}
