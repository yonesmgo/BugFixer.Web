using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BugFixer.Domain.ViewModels.UserPanel.Account
{
    public class EditUserViewModel
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression(@"^([\u0600-\u06FF]+\s?)+$", ErrorMessage = "لطفا فقط از حروف فارسی استفاده بفرمایید ")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [RegularExpression(@"^([\u0600-\u06FF]+\s?)+$", ErrorMessage = "لطفا فقط از حروف فارسی استفاده بفرمایید ")]

        public string LastName { get; set; }
        [Display(Name = "شماره تماس")]
        [RegularExpression(@"^(?:9)?(9\d{9})$", ErrorMessage = "فرمت ورودی شماره تماس اشتباه میباشد ")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? PhoneNumber { get; set; }
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
        [Display(Name = "تاریخ تولد")]
        [RegularExpression(@"^\d{4}/((0[1-9])|(1[012]))/((0[1-9]|[12]\d)|3[01])$", ErrorMessage = "فرمت ورودی تاریخ اشتباه میباشد")]
        public string? BirthDate { get; set; }
        [Display(Name = "استان")]
        public long? CountryId { get; set; }
        [Display(Name = "شهر")]
        public long? CityId { get; set; }
        public bool GetNewsLetter { get; set; }
    }
    public enum EditUserResult
    {
        Success, NotValidate,
    }
}
