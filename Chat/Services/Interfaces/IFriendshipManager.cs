using ChatAPI.Hubs;
using ChatAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IFriendshipManager
    {
        public Task<bool> SendFriendRequestAsync(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, Guid requestorUserId, Guid respondentUserId);
        public Task<bool> AcceptFriendRequestAsync(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, Guid currentUserId, Guid friendshipId);
        public Task<IEnumerable<Friendship>> GetFriendsAsync(Guid userId);
        public Task<IEnumerable<Friendship>> GetPendingFriendRequestsAsync(Guid userId);
        public Task<IEnumerable<Friendship>> GetRecentlyAcceptedRequestsAsync(Guid currentUserId);
        public Task RequestDeliveredAsync(Guid friendshipId);
        public Task AcceptanceNotifiedAsync(Guid friendshipId);
    }
}
