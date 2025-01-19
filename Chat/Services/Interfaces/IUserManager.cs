using ChatAPI.Models;

namespace ChatAPI.Services.Interfaces
{
    public interface IUserManager
    {
        Task<User> GetUserAsync(string userName, string password);
        Task CreateUserAsync(User user);
        Task<bool> UserNameExistsAsync(string userName);
    }
}
