using ChatAPI.Dtos;
using ChatAPI.Models;

namespace ChatAPI.Services.Interfaces
{
    public interface IGroupMessageManager
    {
        Task<IEnumerable<GroupMessage>> GetPendingPrivateMessagesAsync(Guid userId);
        Task MessageDeliveredAsync(Guid privateMessageId, Guid userId);
        Task GroupMessageingAsync(Guid currentUserId, GroupMessageDto messageDto);
    }
}
