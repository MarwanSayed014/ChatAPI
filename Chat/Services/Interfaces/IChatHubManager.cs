using ChatAPI.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IChatHubManager
    {

        public Task OnConnectedAsync(Guid currentUserId, string connectionId);
        public Task OnDisconnectedAsync(Guid currentUserId, string connectionId);
    }
}
