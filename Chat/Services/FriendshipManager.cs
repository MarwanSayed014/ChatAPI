using ChatAPI.Dtos;
using ChatAPI.Hubs;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;
using ChatAPI.Types;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Services
{
    public class FriendshipManager : IFriendshipManager
    {
        public IFriendshipRepo _friendshipRepo { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }

        public FriendshipManager(IFriendshipRepo friendshipRepo, IUserConnectionsManager userConnectionsManager)
        {
            _friendshipRepo = friendshipRepo;
            _userConnectionsManager = userConnectionsManager;
        }


        public async Task<bool> AcceptFriendRequestAsync(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, Guid currentUserId, Guid friendshipId)
        {
            var friendship = (await _friendshipRepo.FindAsync(x => x.FriendshipId == friendshipId)).
                Include(x=> x.RequestorUser).Include(x=> x.RespondentUser).SingleOrDefault();
            if (friendship == null) return false;
            if (friendship.RespondentUserId != currentUserId) return false;
            if (friendship.FriendshipStatus != FriendshipStatus.Pending) return false;

            friendship.FriendshipStatus = FriendshipStatus.Friends;
            await _friendshipRepo.UpdateAsync(friendship);
            await _friendshipRepo.SaveAsync();

            var RespondentUserIds = await _userConnectionsManager.GetUserConnectionsId(currentUserId);
            foreach (var conId in RespondentUserIds)
            {
                await hubContext.Clients.Client(conId).SendAsync("AcceptFriendShipRequest", friendship.RespondentUserId, $"{friendship.RespondentUserId} Accept {friendship.RequestorUserId} friend request");
                await hubContext.Groups.AddToGroupAsync(conId, friendship.FriendshipId.ToString());
            }


            var RequestorUserIds = await _userConnectionsManager.GetUserConnectionsId(friendship.RequestorUserId);
            foreach (var conId in RequestorUserIds)
            {
                await hubContext.Clients.Client(conId).SendAsync("AcceptFriendShipRequest", friendship.RespondentUserId, $"{friendship.RespondentUserId} Accept your friend request");
                await hubContext.Groups.AddToGroupAsync(conId, friendship.FriendshipId.ToString());
            }

            if(RespondentUserIds?.Count() > 0)
                await hubContext.Clients.Groups(friendship.FriendshipId.ToString()).SendAsync("userstatus", friendship.RespondentUser.Name, "Online");

            if (RequestorUserIds?.Count() > 0)
                await hubContext.Clients.Groups(friendship.FriendshipId.ToString()).SendAsync("userstatus", friendship.RequestorUser.Name, "Online");

            if(RequestorUserIds?.Count() == 0)
            {
                friendship.NotifyAcceptance = true;
                _friendshipRepo.UpdateAsync(friendship);
                _friendshipRepo.SaveAsync();
            }

            return true;
        }

        public async Task<bool> SendFriendRequestAsync(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, Guid requestorUserId, Guid respondentUserId)
        {
            if(requestorUserId == respondentUserId) return false;

            var friendship = (await _friendshipRepo.FindAsync(x =>
            (x.RequestorUserId == requestorUserId && x.RespondentUserId == respondentUserId)
            || (x.RequestorUserId == respondentUserId && x.RespondentUserId == requestorUserId)
            )).Include(x => x.RequestorUser).Include(x => x.RespondentUser).SingleOrDefault();

            if(friendship != null) return false;

            friendship = new Friendship
            {
                RequestorUserId = requestorUserId,
                RespondentUserId = respondentUserId,
                FriendshipStatus = FriendshipStatus.Pending,
                FriendshipId = Guid.NewGuid(),
            };
            await _friendshipRepo.CreateAsync(friendship);
            await _friendshipRepo.SaveAsync();

            var RespondentUserIds = await _userConnectionsManager.GetUserConnectionsId(friendship.RespondentUserId);
            foreach (var conId in RespondentUserIds)
            {
                await hubContext.Clients.Client(conId).SendAsync("FriendShipRequest", friendship.FriendshipId, $"{friendship.RequestorUserId} Send Friend Request To You");
            }

            if (RespondentUserIds?.Count() > 0)
            {
                friendship.IsDelivered = true;
                await _friendshipRepo.UpdateAsync(friendship);
                await _friendshipRepo.SaveAsync();
            } 

            var RequestorUserIds = await _userConnectionsManager.GetUserConnectionsId(friendship.RequestorUserId);
            foreach (var conId in RequestorUserIds)
            {
                await hubContext.Clients.Client(conId).SendAsync("FriendShipRequest", friendship.FriendshipId, $"Your Sent a Friend Request To {friendship.RespondentUserId}");
            }

            return true;
        }

        public async Task<IEnumerable<Friendship>> GetFriendsAsync(Guid userId)
        {
            return (await _friendshipRepo.GetFriendsAsync(userId)).ToList();
        }

        public async Task<IEnumerable<Friendship>> GetPendingFriendRequestsAsync(Guid userId)
        {
            return (await _friendshipRepo.GetPendingFriendRequestsAsync(userId)).ToList();
        }

        public async Task<IEnumerable<Friendship>> GetRecentlyAcceptedRequestsAsync(Guid userId)
        {
            return (await _friendshipRepo.GetRecentlyAcceptedRequestAsync(userId)).ToList();
        }

        public async Task RequestDeliveredAsync(Guid friendshipId)
        {
            Friendship friendship = (await _friendshipRepo.FindAsync(x=> x.FriendshipId == friendshipId)).
                SingleOrDefault();

            if(friendship != null)
            {
                friendship.IsDelivered = true;
                await _friendshipRepo.UpdateAsync(friendship);
                await _friendshipRepo.SaveAsync();
            }
        }

        public async Task AcceptanceNotifiedAsync(Guid friendshipId)
        {
            var friendship = (await _friendshipRepo.FindAsync(x => x.FriendshipId == friendshipId))
                                .SingleOrDefault();
            if(friendship != null)
            {
                if (friendship.FriendshipStatus == FriendshipStatus.Friends)
                {
                    friendship.NotifyAcceptance = false;
                    await _friendshipRepo.UpdateAsync(friendship);
                    await _friendshipRepo.SaveAsync();
                }
                
            }
            
        }
    }
}
