using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Statics
{
    public static class PathTools
    {
        #region User
        public static readonly string DefaultUserAvatar = "DefaultAvatar.png";
        public static readonly string USerAvatarServerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/user/");
        public static readonly string USerAvatarPath = "/content/user/";
        #endregion
        #region Site
        public static readonly string SiteAddress = "https://localhost:7130";
        #endregion
        #region Ck Editor 
        public static readonly string EditorImageServerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/CkEditor/");
        public static readonly string EditorImagePath = "/content/CkEditor/";

        #endregion


    }
}
