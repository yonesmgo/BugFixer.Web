using BugFixer.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.ViewComponents
{
    public class QuestionAnswerListViewComponent : ViewComponent
    {
  
        private IQuestionServices _questionServices;
        #region Ctor
        public QuestionAnswerListViewComponent(IQuestionServices questionServices)
        {
            _questionServices = questionServices;
        }
        #endregion
        public async Task<IViewComponentResult> InvokeAsync(long questionID)
        {
            var answer = await _questionServices.GetallQuestionAnswer(questionID);
            return View("QuestionAnswerList", answer);
        }
    }
}
