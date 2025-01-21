using ChatAPI.Models;

namespace ChatAPI.Repos.Interfaces
{
    public interface IUserPrivateChatRepo : IRepo<UserPrivateChat>
    {
        Task AddUsersToPrivateChat(IEnumerable<Guid> usersIds, Guid chatId);
        Task<IEnumerable<PrivateChat>> GetAllUserPrivateChatsAsync(Guid userId);
        Task<PrivateChat> GetPrivateChatAsync(Guid currentUserId, Guid anotherUserId);
        Task<IEnumerable<PrivateChat>> GetPrivateChatAsync(Guid userId);
    }
}
