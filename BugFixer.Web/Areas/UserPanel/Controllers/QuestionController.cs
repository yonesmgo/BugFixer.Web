using BugFixer.Application.Extentions;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.UserPanel.Question;
using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Areas.UserPanel.Controllers
{
    public class QuestionController : UserPanelBaseController
    {
        #region Ctor
        private readonly IQuestionServices _questionServices;
        public QuestionController(IQuestionServices questionServices)
        {
            _questionServices = questionServices;
        }
        #endregion
        #region Bookmarks
        public async Task<IActionResult> QuestionBookmarks(FilterQuestionBooknarksViewModel filter)
        {
            filter.UserId = User.GetUserId();
            filter.TakeEntity = 3;
            var res = await _questionServices.FilterQuestionBooknarks(filter);
            return View(res);
        }
        #endregion
        #region Question By User
        public async Task<IActionResult> QuestionByUser(FilterQuestionBooknarksViewModel filter)
        {
            filter.UserId = User.GetUserId();
            filter.TakeEntity = 3;
            var res = await _questionServices.GetAllBokkmarksByUser(filter);
            return View(res);
        }
        #endregion
    }
}
