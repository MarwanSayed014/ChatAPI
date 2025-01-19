using ChatAPI.Dtos;
using ChatAPI.Models;

namespace ChatAPI.Services.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<PrivateChat>> GetAllUserPrivateChatsAsync(Guid userId);
        Task<IEnumerable<PrivateMessage>> GetPendingPrivateMessagesAsync(Guid userId);
        Task MessageDeliveredAsync(Guid privateMessageId);
        Task<(PrivateChat privateChat, PrivateMessage PrivateMessage)> PrivateMessageingAsync(Guid CurrentUserId, PrivateMessageDto messageDto);
    }
}
