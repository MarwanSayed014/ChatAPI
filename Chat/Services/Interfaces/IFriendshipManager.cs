using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services.Interfaces
{
    public interface IFriendshipManager
    {
        public Task<bool> SendFriendRequestAsync(IHubClients clients, IGroupManager groups, Guid requestorUserId, Guid respondentUserId);
        public Task<bool> AcceptFriendRequestAsync(IHubClients clients, IGroupManager groups, Guid currentUserId, Guid friendshipId);
    }
}
