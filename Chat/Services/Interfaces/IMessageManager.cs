using ChatAPI.Dtos;
using ChatAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IMessageManager
    {
        Task PrivateMessageingAsync(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, Guid currentUserId, PrivateMessageDto messageDto);
    }
}
