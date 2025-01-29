using ChatAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IFriendshipManager
    {
        public Task<bool> SendFriendRequestAsync(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, Guid requestorUserId, Guid respondentUserId);
        public Task<bool> AcceptFriendRequestAsync(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, Guid currentUserId, Guid friendshipId);
    }
}
