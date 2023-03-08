using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Linq;

namespace BugFixer.DataLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BugFixerDbContext _context;
        #region ctor
        public UserRepository(BugFixerDbContext context)
        {
            _context = context;
        }
        #endregion
        public async Task<bool> isExistUserByEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task CreateUSer(User user)
        {
            await _context.AddAsync(user);
        }
        public async Task save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetuserByEMail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        #region Activation Code Return 
        public async Task<User> GetUserByActivationCode(string activationCode)
        {
            return await _context.Users.FirstOrDefaultAsync(a => a.EmailActivationCode.Equals(activationCode));
        }

        public async Task Update(User user)
        {
            _context.Update(user);
        }

        public async Task<User> getUserbyID(long userid)
        {
            var user = await _context.Users.FindAsync(userid);
            return user;
        }
        #endregion
    }
}
