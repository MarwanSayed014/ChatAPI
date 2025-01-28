using ChatAPI.Dtos;
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


        public async Task<bool> AcceptFriendRequestAsync(IHubClients clients, IGroupManager groups, Guid currentUserId, Guid friendshipId)
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
                await clients.Client(conId).SendAsync("AcceptFriendShipRequest", friendship.RespondentUser.Id, $"{friendship.RespondentUser.Name} Accept {friendship.RequestorUser.Name} friend request");
            }


            var RequestorUserIds = await _userConnectionsManager.GetUserConnectionsId(friendship.RequestorUserId);
            foreach (var conId in RequestorUserIds)
            {
                await clients.Client(conId).SendAsync("AcceptFriendShipRequest", friendship.RespondentUser.Id, $"{friendship.RespondentUser.Name} Accept your friend request");
            }


            return true;
        }

        public async Task<bool> SendFriendRequestAsync(IHubClients clients, IGroupManager groups, Guid requestorUserId, Guid respondentUserId)
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
                FriendshipId = Guid.NewGuid()
            };
            await _friendshipRepo.CreateAsync(friendship);
            await _friendshipRepo.SaveAsync();

            var RespondentUserIds = await _userConnectionsManager.GetUserConnectionsId(friendship.RespondentUserId);
            foreach (var conId in RespondentUserIds)
            {
                await groups.AddToGroupAsync(conId, friendship.FriendshipId.ToString());
            }


            var RequestorUserIds = await _userConnectionsManager.GetUserConnectionsId(friendship.RequestorUserId);
            foreach (var conId in RequestorUserIds)
            {
                await groups.AddToGroupAsync(conId, friendship.FriendshipId.ToString());
            }
            await clients.Group(friendship.FriendshipId.ToString()).SendAsync("FriendShipRequest", friendship.FriendshipId, $"{friendship.RequestorUser.Name} Send Friend Request To You");
            await clients.Group(friendship.FriendshipId.ToString()).SendAsync("FriendShipRequest", friendship.FriendshipId, $"Your Sent a Friend Request To {friendship.RespondentUser.Name}");


            return true;
        }
    }
}
