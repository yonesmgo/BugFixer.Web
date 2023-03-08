using AngleSharp.Dom;
using BugFixer.Application.Extentions;
using BugFixer.Application.Securities;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BugFixer.Web.Controllers
{
    public class QuestionController : BaseController
    {

        #region ctor
        private readonly IQuestionServices _services;
        public QuestionController(IQuestionServices services)
        {
            _services = services;
        }
        #endregion
        #region Create Question 
        [Authorize]
        [HttpGet("Create-Question")]
        public async Task<IActionResult> CreateQuestion()
        {
            return View();
        }
        [Authorize]
        [HttpPost("Create-Question"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(CreateQuestionViewModel model)
        {
            //if (model.Tags is null || !model.Tags.Any())
            //{
            //    TempData[WarningMessage] = "لطفا تگهای مورد نظر را برای سوال قرار دهید ";
            //    return View(model);
            //}
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            var tagResult = await _services.CheckTagValidation(model.Tags, HttpContext.User.GetUserId());
            if (tagResult.Status == CreateQuestionResultEnum.NotValidTag)
            {
                model.SelectedTagJson = JsonConvert.SerializeObject(model.Tags);
                model.Tags = null;
                TempData[errorMessage] = tagResult.Message;
                return View(model);
            }

            model.UserID = HttpContext.User.GetUserId();
            var res = await _services.CreateQuestion(model);
            if (res)
            {
                TempData[successMessage] = "عملیات با موفقیت  انجام شد ";
                return Redirect("/");
            }
            model.SelectedTagJson = JsonConvert.SerializeObject(model.Tags);
            model.Tags = null;
            return View(model);
        }
        #endregion
        #region GetTag
        [Authorize]
        [HttpGet("get-tags")]
        public async Task<IActionResult> getTags(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(null);
            }
            var RES = await _services.GetTags();
            var filtered = RES.Where(s => s.Title.Contains(name)).Select(a => a.Title).ToList();
            return Json(filtered);
        }
        #endregion
        #region Question List

        [HttpGet("Question-List")]
        public async Task<IActionResult> QuestionList(FilterQuestionViewModel filter)
        {
            var result = await _services.FilterQuestion(filter);
            return View(result);
        }
        #endregion
        #region Filter Question By Tag
        [HttpGet("Tags/{tagname}")]
        public async Task<IActionResult> QuestionListByTag(FilterQuestionViewModel filter, string tagname)
        {
            tagname = tagname.Trim().ToLower().SanitizeText();
            filter.TagTitle = tagname;
            ViewBag.tagname = tagname;
            var result = await _services.FilterQuestion(filter);
            return View(result);
        }
        #endregion
        #region Filter Tags List
        [HttpGet("Tags")]
        public async Task<IActionResult> FilterTAgs(FilterTagViewModel filter)
        {
            filter.TakeEntity = 12;
            var result = await _services.FilterTags(filter);
            return View(result);
        }

        #endregion
        #region Question Details
        [HttpGet("Question-Details/{QuestionId}")]
        public async Task<IActionResult> QuestionDetails(long QuestionId)
        {
            var question = await _services.GetQuestionByID(QuestionId);
            if (question is null) return NotFound();
            ViewData["TagList"] = await _services.GetTaglistByQuestionId(QuestionId);
            return View(question);
        }
        #endregion
    }
}
