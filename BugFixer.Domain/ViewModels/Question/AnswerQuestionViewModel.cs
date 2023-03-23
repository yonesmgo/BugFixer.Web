using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.ViewModels.Question
{
    public class AnswerQuestionViewModel
    {
        public string Answer { get; set; }
        public long QuestionID { get; set; }
        public long UserID { get; set; }
    }
}
