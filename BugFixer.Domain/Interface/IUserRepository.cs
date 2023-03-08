using BugFixer.Domain.Entities.Account;

namespace BugFixer.Domain.Interface
{
    public interface IUserRepository
    {
        Task<bool> isExistUserByEmail(string email);
        Task save();
        Task Update(User user);
        Task CreateUSer(User user);
        Task<User> GetuserByEMail(string email);
        Task<User> GetUserByActivationCode(string activationCode);
        Task<User> getUserbyID(long userid);
    }
}
