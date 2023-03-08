using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.SiteSetting;
using BugFixer.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace BugFixer.DataLayer.Repositories
{
    public class SettingSiteReporistory : ISettingSiteReporistory
    {
        #region Ctor
        private readonly BugFixerDbContext _context;
        public SettingSiteReporistory(BugFixerDbContext context)
        {
            _context = context;
        }


        #endregion
        #region Email
        public async Task<EmailSetting> GetDefaultEmail()
        {
            return await _context.EmailSettings.FirstOrDefaultAsync(s=>!s.IsDelete && s.IsDefault);
        }
        #endregion
    }
}
