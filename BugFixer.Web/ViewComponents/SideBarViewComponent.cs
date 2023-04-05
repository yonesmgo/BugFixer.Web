using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.ViewComponents
{
    public class ScoreDesQuestionViewComponent : ViewComponent
    {
        #region Ctor
        private IQuestionServices _questionServices;
        public ScoreDesQuestionViewComponent(IQuestionServices questionServices)
        {
            _questionServices = questionServices;
        }
        #endregion
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var option = new FilterQuestionViewModel
            {
                TakeEntity = 10,
                sort = FilterQuestuionSortEnum.ScoreHighToLow,

            };
            var result = await _questionServices.FilterQuestion(option);
            return View("ScoreDesQuestion", result);
        }
    }
    public class CreateDateQuestionViewComponent : ViewComponent
    {
        #region Ctor
        private IQuestionServices _questionServices;
        public CreateDateQuestionViewComponent(IQuestionServices questionServices)
        {
            _questionServices = questionServices;
        }
        #endregion
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var option = new FilterQuestionViewModel
            {
                TakeEntity = 5,
                sort = FilterQuestuionSortEnum.NewToOld,

            };
            var result = await _questionServices.FilterQuestion(option);
            return View("CreateDateQuestion", result);
        }
    }
    public class UseCountTagViewComponent : ViewComponent
    {
        #region Ctor
        private IQuestionServices _questionServices;
        public UseCountTagViewComponent(IQuestionServices questionServices)
        {
            _questionServices = questionServices;
        }
        #endregion
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var option = new FilterTagViewModel
            {
                TakeEntity = 10,
                Sort = FilterTagEnum.UseCountHighToLow,
            };
            var result = await _questionServices.FilterTags(option);
            return View("UseCountTag", result);
        }
    }
}
