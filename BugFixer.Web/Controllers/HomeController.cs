using BugFixer.Application.Extentions;
using BugFixer.Application.Services.Implimentations;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BugFixer.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IQuestionServices _questionServices;
        public HomeController(IQuestionServices questionServices)
        {
            _questionServices = questionServices;
        }
        public async Task<IActionResult> Index()
        {
            var option = new FilterQuestionViewModel
            {
                TakeEntity = 5,
                sort = FilterQuestuionSortEnum.NewToOld,

            };
            ViewData["Questions"] = await _questionServices.FilterQuestion(option);
            return View();
        }

        #region Editor Upload
        public async Task<IActionResult> UploadEditorImage(IFormFile upload)
        {
            var filename = Guid.NewGuid() + Path.GetExtension(upload.FileName);
            upload.UploadFile(filename, PathTools.EditorImageServerPath);

            return Json(new { url = $"{PathTools.EditorImagePath}{filename}" });
        }
        #endregion

    }
}