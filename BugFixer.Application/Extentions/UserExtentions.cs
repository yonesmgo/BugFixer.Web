using BugFixer.Domain.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Extentions
{
    public static class UserExtentions
    {
        public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var identifier = claimsPrincipal.Claims.SingleOrDefault(s => s.Type == ClaimTypes.NameIdentifier);
            if (identifier == null) return 0;
            return long.Parse(identifier.Value);
        }

        public static string GetUserDisplayname(this User user)
        {
            if (!string.IsNullOrEmpty(user.FirstName) || !string.IsNullOrEmpty(user.LastName))
            {
                return $"{user.FirstName} {user.LastName}";
            }
            return user.Email.Split("@")[0];

        }
        public static string GetUserName(this User user)
        {
            if (!string.IsNullOrEmpty(user.Email))
            {
                return $"{user.Email.Replace("@mtnirancell.ir", "")}";
            }
            return user.Email.Split("@")[0];

        }
    }
}
