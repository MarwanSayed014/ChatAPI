using ChatAPI.Dtos;
using ChatAPI.Hubs;
using ChatAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IPrivateMessageManager
    {
        Task<IEnumerable<PrivateMessage>> GetPendingPrivateMessagesAsync(Guid userId);
        Task MessageDeliveredAsync(Guid privateMessageId);
        Task PrivateMessageingAsync(Guid currentUserId, PrivateMessageDto messageDto);
    }
}
