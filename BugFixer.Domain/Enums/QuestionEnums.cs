using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.Enums
{
    public enum QuestionScoreType
    {
        [Display(Name = "مثبت")]
        plus,
        [Display(Name = "منفی")]
        Mines,
    }
}
