using ChatAPI.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IChatHubManager
    {
        //public Task PrivateMessageingAsync(dynamic clients, IGroupManager groups,
        //                                        Guid currentUserId, PrivateMessageDto messageDto);
        //public Task PrivateMessageingAsync(IHubCallerClients clients, IGroupManager groups,
        //                                        Guid currentUserId, PrivateMessageDto messageDto);
        public Task OnConnectedAsync(IHubCallerClients clients, IGroupManager groups,
                                                  Guid currentUserId, string connectionId);
        public Task OnDisconnectedAsync(IHubCallerClients clients, IGroupManager groups,
                                                  Guid currentUserId, string connectionId);
        Task PrivateMessageingAsync(IHubClients clients, IGroupManager groups, Guid currentUserId, PrivateMessageDto messageDto);
    }
}
