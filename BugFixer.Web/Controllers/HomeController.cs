using BugFixer.Application.Extentions;
using BugFixer.Application.Statics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BugFixer.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
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