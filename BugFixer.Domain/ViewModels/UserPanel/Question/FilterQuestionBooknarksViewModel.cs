using BugFixer.Domain.ViewModels.Question;

namespace BugFixer.Domain.ViewModels.UserPanel.Question
{
    public class FilterQuestionBooknarksViewModel:Paging<Domain.Entities.Questions.Question>
    {
        public long UserId { get; set; }
    }
}
