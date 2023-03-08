using BugFixer.Domain.Entities.Questions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace BugFixer.Domain.ViewModels.Question
{
    public class FilterQuestionViewModel : Paging<QuestionListViewModel>
    {
        public string? Title { get; set; }
        public FilterQuestuionSortEnum sort { get; set; }

        public string? TagTitle { get; set; }
    }
    public enum FilterQuestuionSortEnum
    {
        [Display(Name = "تاریخ ثبت نزولی")]
        NewToOld,
        [Display(Name = "تاریخ ثبت صعودی")]
        OldToNew,
        [Display(Name = "امتیاز صعودی")]
        ScoreHighToLow,
        [Display(Name = "امتیاز نزولی")]
        ScoreLowToHigh,
    }
}
