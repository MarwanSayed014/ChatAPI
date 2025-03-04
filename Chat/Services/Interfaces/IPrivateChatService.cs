using ChatAPI.Dtos;
using ChatAPI.Models;

namespace ChatAPI.Services.Interfaces
{
    public interface IPrivateChatService
    {
        Task<IEnumerable<PrivateChat>> GetAllUserPrivateChatsAsync(Guid userId);
        Task<PrivateChat> GetPrivateChatAsync(Guid currentUserId, Guid anotherUserId);
        Task CreatePrivateChat(PrivateChat privateChat, Guid currentUserId, Guid anotherUserId);
    }
}
