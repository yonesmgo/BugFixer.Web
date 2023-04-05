using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BugFixer.Domain.ViewModels.Question
{
    public class EditQuestionViewModel
    {
        [Display(Name = "عنوان")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }
        [Display(Name = "تگ ها ")]
        public List<string>? Tags { get; set; }
        public string? SelectedTagJson { get; set; }
        public long UserID { get; set; }
        public long Id { get; set; }
    }
}
