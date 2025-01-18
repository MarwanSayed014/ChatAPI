using ChatAPI.Models;

namespace ChatAPI.Repos.Interfaces
{
    public interface IUserRepo : IRepo<User>
    {
        Task<bool> UserExists(Guid userId);
        Task<bool> UserNameExistsAsync(string userName);
    }
}
