using ChatAPI.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IChatHubManager
    {

        public Task OnConnectedAsync(IHubCallerClients clients, IGroupManager groups,
                                                  Guid currentUserId, string connectionId);
        public Task OnDisconnectedAsync(IHubCallerClients clients, IGroupManager groups,
                                                  Guid currentUserId, string connectionId);
    }
}
