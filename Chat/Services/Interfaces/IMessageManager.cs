using ChatAPI.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IMessageManager
    {
        Task PrivateMessageingAsync(IHubClients clients, IGroupManager groups, Guid currentUserId, PrivateMessageDto messageDto);
    }
}
