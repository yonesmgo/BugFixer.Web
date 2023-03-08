using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.ViewModels.Question
{
    public class QuestionListViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string UserDisplayName { get; set; }
        public string CreatedDate { get; set; }
        public bool HasAnyAnswer { get; set; }
        public bool HasTrueAnswer { get; set; }
        public int AnswerSCount { get; set; }
        public int Score { get; set; }
        public int ViewCount { get; set; }
        public string? AnswerByDisplayName { get; set; }
        public string? AnswerByCreatedDate { get; set; }
        public List<string> TagsList { get; set; }
    }
}
