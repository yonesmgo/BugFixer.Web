using AngleSharp.Dom;
using BugFixer.Application.Extentions;
using BugFixer.Application.Securities;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Enums;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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
            //if (!ModelState.IsValid)
            //{
            //    model.SelectedTagJson = JsonConvert.SerializeObject(model.Tags);
            //    model.Tags = null;
            //    TempData[errorMessage] = "اطلاعات ورودی شما نا معتبر میباشد";
            //    return View(model);
            //}
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
        #region Edit Question
        [HttpGet("edit-question/{id}")]
        [Authorize]
        public async Task<IActionResult> EditQuestion(long ID)
        {
            var res = await _services.FillEditQuestionViewModel(ID, User.GetUserId());
            if (res is null) return NotFound();
            return View(res);
        }
        [HttpPost("edit-question/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditQuestion(EditQuestionViewModel edit)
        {

            var tagResult = await _services.CheckTagValidation(edit.Tags, HttpContext.User.GetUserId());
            if (tagResult.Status == CreateQuestionResultEnum.NotValidTag)
            {
                edit.SelectedTagJson = JsonConvert.SerializeObject(edit.Tags);
                edit.Tags = null;
                TempData[errorMessage] = tagResult.Message;
                return View(edit);
            }
            if (!ModelState.IsValid)
            {
                edit.SelectedTagJson = JsonConvert.SerializeObject(edit.Tags);
                edit.Tags = null;
                TempData[errorMessage] = "اطلاعات ورودی شما نا معتبر میباشد";
                return View(edit);
            }
            edit.UserID = HttpContext.User.GetUserId();
            bool res = await _services.EditQuestion(edit);
            if (res)
            {
                TempData[successMessage] = "عملیات با موفقیت  انجام شد ";
                return Redirect("/");
            }
            edit.SelectedTagJson = JsonConvert.SerializeObject(edit.Tags);
            edit.Tags = null;
            return View(edit);
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

        #region Get Question Ajax
        [HttpGet("get-question")]
        public async Task<IActionResult> GetQuestionAjax(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(null);
            }
            var RES = await _services.GetQuestions();
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
            var userip = Request.HttpContext.Connection.RemoteIpAddress;
            // var username = HttpContext.User.GetUserId();
            if (User != null)
            {
                await _services.addViewForQuestion(userip.ToString(), question);
            }
            ViewBag.IsBookmarks = false;
            if (question is null) return NotFound();
            if (User.Identity.IsAuthenticated && await _services.isExistInUserBookmarks(QuestionId, User.GetUserId()))
            {
                ViewBag.IsBookmarks = true;
            }
            ViewData["TagList"] = await _services.GetTaglistByQuestionId(QuestionId);
            return View(question);
        }


        [HttpGet("q/{QuestionId}")]
        public async Task<IActionResult> QuestionDetailsByShortLink(long QuestionId)
        {
            var question = await _services.GetQuestionByID(QuestionId);
            if (question is null)
            {
                return NotFound();
            }
            return RedirectToAction("QuestionDetails", "Question", new { QuestionId = QuestionId });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AnswerQuestion(AnswerQuestionViewModel model)
        {
            if (string.IsNullOrEmpty(model.Answer))
            {
                return new JsonResult(new { status = "EmptyAnswer" });

            }
            model.UserID = User.GetUserId();
            var res = await _services.AnswerQuestion(model);
            if (res)
            {
                return new JsonResult(new { status = "Success" });

            }
            else
            {
                return new JsonResult(new { status = "Error" });
            }

        }
        [Authorize]
        [HttpGet("Edit-Answer/{AnswerId}")]
        public async Task<IActionResult> EditAnswer(long AnswerId)
        {
            var result = await _services.FillEditANSWERViewModel(AnswerId, User.GetUserId());
            if (result is null) return NotFound();
            return View(result);
        }
        [Authorize]
        [HttpPost("Edit-Answer/{AnswerId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnswer(EditAnswerViewModel edit)
        {
            if (!ModelState.IsValid)
            {
                return View(edit);
            }
            edit.UserID=User.GetUserId();
            if (await _services.EditAnswer(edit)) 
            {
                TempData[successMessage] = "عملیات با وفقیت انجاشم شد";
                return RedirectToAction("QuestionDetails", "Question", new { QuestionId = edit.QuestionId });
            };
            TempData[errorMessage] = "خطایی رخ داده است ";
            return View(edit);
        }

        #endregion 
        #region Select True Answer
        [HttpPost("SelectTrueAnswer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectTrueAnswer(long AnswerId)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult(new
                {
                    status = "Not Athorized"
                });
            }
            if (!await _services.HasUserAccessToSelectTrueAnswer(User.GetUserId(), AnswerId))
            {
                return new JsonResult(new
                {
                    status = "Not Access"
                });
            }
            await _services.SelectTrueAnswer(AnswerId);
            return new JsonResult(new { status = "Success" });
        }



        #endregion
        #region Scoree Answer
        [HttpPost("ScoreUpForAnswer")]
        public async Task<IActionResult> ScoreUpForAnswer(long answerId)
        {
            var res = await _services.CreateScoreForAnswer(answerId, AnswerScoreType.plus, User.GetUserId());
            switch (res)
            {
                case CreateScoreForAnswerResult.error:
                    return new JsonResult(new { status = "Error" });

                case CreateScoreForAnswerResult.notEnoghScoreForDown:
                    return new JsonResult(new { status = "notEnoghScoreForDown" });

                case CreateScoreForAnswerResult.notEnoghScoreForUp:
                    return new JsonResult(new { status = "notEnoghScoreForUp" });

                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });

                case CreateScoreForAnswerResult.success:
                    return new JsonResult(new { status = "success" });

            }
            return View();
        }
        [HttpPost("ScoreDownForAnswer")]
        public async Task<IActionResult> ScoreDownForAnswer(long answerId)
        {
            var res = await _services.CreateScoreForAnswer(answerId, AnswerScoreType.Mines, User.GetUserId());
            switch (res)
            {
                case CreateScoreForAnswerResult.error:
                    return new JsonResult(new { status = "Error" });

                case CreateScoreForAnswerResult.notEnoghScoreForDown:
                    return new JsonResult(new { status = "notEnoghScoreForDown" });

                case CreateScoreForAnswerResult.notEnoghScoreForUp:
                    return new JsonResult(new { status = "notEnoghScoreForUp" });

                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });

                case CreateScoreForAnswerResult.success:
                    return new JsonResult(new { status = "success" });
            }
            return View();
        }
        #endregion
        #region Scoree Question
        [HttpPost("ScoreUpForQuestion")]
        public async Task<IActionResult> ScoreUpForQuestion(long QuestionId)
        {
            var res = await _services.CreateScoreForQuestion(QuestionId, QuestionScoreType.plus, User.GetUserId());
            switch (res)
            {
                case CreateScoreForAnswerResult.error:
                    return new JsonResult(new { status = "Error" });

                case CreateScoreForAnswerResult.notEnoghScoreForDown:
                    return new JsonResult(new { status = "notEnoghScoreForDown" });

                case CreateScoreForAnswerResult.notEnoghScoreForUp:
                    return new JsonResult(new { status = "notEnoghScoreForUp" });

                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });

                case CreateScoreForAnswerResult.success:
                    return new JsonResult(new { status = "success" });

            }
            return View();
        }
        [HttpPost("ScoreDownForQuestion")]
        public async Task<IActionResult> ScoreDownForQuestion(long QuestionId)
        {
            var res = await _services.CreateScoreForQuestion(QuestionId, QuestionScoreType.Mines, User.GetUserId());
            switch (res)
            {
                case CreateScoreForAnswerResult.error:
                    return new JsonResult(new { status = "Error" });

                case CreateScoreForAnswerResult.notEnoghScoreForDown:
                    return new JsonResult(new { status = "notEnoghScoreForDown" });

                case CreateScoreForAnswerResult.notEnoghScoreForUp:
                    return new JsonResult(new { status = "notEnoghScoreForUp" });

                case CreateScoreForAnswerResult.UserCreateScoreBefore:
                    return new JsonResult(new { status = "UserCreateScoreBefore" });

                case CreateScoreForAnswerResult.success:
                    return new JsonResult(new { status = "success" });
            }
            return View();
        }
        #endregion
        #region AddQuestionToBookmark
        [HttpPost("AddQuestionToBookmark")]
        public async Task<IActionResult> AddQuestionToBookmark(long QuestionId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult(new
                {
                    status = "Not Athorized"
                });
            }
            var res = await _services.AddQuestionToBookmark(QuestionId, User.GetUserId());
            if (!res)
            {
                return new JsonResult(new
                {
                    status = "Error"
                });
            }
            return new JsonResult(new
            {
                status = "Success"
            });
        }
        #endregion
    }
}
