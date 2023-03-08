using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace BugFixer.Application.Extentions
{
    public static class FileExtentions
    {
        public static bool UploadFile(this IFormFile file, string filename, string path
            , List<string>? ValidFormat = null)
        {
            if (ValidFormat != null && ValidFormat.Any())
            {
                var fileformat = Path.GetExtension(file.FileName);
                if (ValidFormat.All(s => s != fileformat))
                {
                    return false;
                }
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var finalPath = path + filename;
            using (var strem = new FileStream(finalPath, FileMode.Create))
            {
                file.CopyTo(strem);
            }
            return true;
        }
    }
}

