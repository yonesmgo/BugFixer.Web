using BugFixer.Application.Statics;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


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

        public static void DeleteFile(this string filename, string path)
        {
            var finalpath = path + filename;
            if (File.Exists(finalpath))
            {
                File.Delete(finalpath);
            }
        }


        public static List<string> GetSrcValue(this string text)
        {
            List<string> imgScrs = new List<string>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);//or doc.Load(htmlFileStream)
            var nodes = doc.DocumentNode.SelectNodes(@"//img[@src]");
            if (nodes is not null && nodes.Any())
            {
                foreach (var img in nodes)
                {
                    HtmlAttribute att = img.Attributes["src"];
                    imgScrs.Add(att.Value.Split("/").Last());
                }
            }
            return imgScrs;
        }
        public static void ManageEditorImages(string curentText, string newText,string path)
        {
            var currentsrc = curentText.GetSrcValue();
            var newSrce = newText.GetSrcValue();
            if (currentsrc.Count() == 0) return;
            if(newSrce.Count() is 0)
            {
                foreach (var img in currentsrc)
                {
                    img.DeleteFile(path);
                }
            }
            foreach (var img in currentsrc)
            {
                if (newSrce.All(s => s != img))
                {
                    img.DeleteFile(path);
                }
            }
        }
    }
}

