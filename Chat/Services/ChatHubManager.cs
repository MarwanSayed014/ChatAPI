using ChatAPI.Dtos;
using ChatAPI.Hubs;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;
using XAct;

namespace ChatAPI.Services
{
    public class ChatHubManager : IChatHubManager
    {
        public IPrivateChatService _privateChatService { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }
        public IUserRepo _userRepo { get; }
        public IFriendshipManager _friendshipManager { get; }
        public IPrivateMessageManager _messageManager { get; }

        public Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> _hubContext { get; set; }

        public ChatHubManager(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, IPrivateChatService privateChatService, IUserConnectionsManager userConnectionsManager,
            IUserRepo userRepo, IFriendshipManager friendshipManager, IPrivateMessageManager messageManager)
        {
            _hubContext = hubContext;
            _privateChatService = privateChatService;
            _userConnectionsManager = userConnectionsManager;
            _userRepo = userRepo;
            _friendshipManager = friendshipManager;
            _messageManager = messageManager;
        }
        public async Task OnConnectedAsync(Guid currentUserId, string connectionId)
        {
            string currentUserName = await _userRepo.GetUserName(currentUserId);
            List<PrivateChat> userPrivateChats = _privateChatService.GetAllUserPrivateChatsAsync(currentUserId).WaitAndGetResult().ToList();
            List<Friendship> friends = _friendshipManager.GetFriendsAsync(currentUserId).WaitAndGetResult().ToList();
            List<Friendship> pendingFriendRequests = _friendshipManager.GetPendingFriendRequestsAsync(currentUserId).WaitAndGetResult().ToList();
            List<Friendship> recentlyAcceptedRequests = _friendshipManager.GetRecentlyAcceptedRequestsAsync(currentUserId).WaitAndGetResult().ToList();
            bool wasOnline = _userConnectionsManager.IsOnline(currentUserId).WaitAndGetResult();


            //Add To Connection Table
            _userConnectionsManager.CreateUserConnectionAsync(new UserConnection
            {
                SignalRConnectionId = connectionId,
                UserId = currentUserId
            }).Wait();

            //Add To All Private Chats
            foreach (var item in userPrivateChats)
            {
                _hubContext.Groups.AddToGroupAsync(connectionId, item.Id.ToString()).Wait();
            }

            //add to all groups




            //Create Online Group And Add User To It And Notify
            foreach (var item in friends)
            {
                _hubContext.Groups.AddToGroupAsync(connectionId, item.FriendshipId.ToString()).Wait();
                if (!wasOnline)
                {
                    _hubContext.Clients.Group(item.FriendshipId.ToString()).SendAsync("userstatus", currentUserName, "Online").Wait();
                }
            }

            foreach(var item in pendingFriendRequests)
            {
                _hubContext.Clients.Client(connectionId).SendAsync("FriendRequestsDelivered", item.FriendshipId, "You have a new friend Request").Wait();
                _friendshipManager.RequestDeliveredAsync(item.FriendshipId).Wait();
            }

            foreach(var item in recentlyAcceptedRequests)
            {
                _hubContext.Clients.Client(connectionId).SendAsync("AcceptFriendShipRequest", item.RespondentUserId, $"{item.RespondentUserId} Accept your friend request").Wait();
                _friendshipManager.AcceptanceNotifiedAsync(item.FriendshipId).Wait();
            }
            

            //Push Pending Private Messages
            List<PrivateMessage> privateMessages = _messageManager.GetPendingPrivateMessagesAsync(currentUserId).WaitAndGetResult().ToList();
            foreach (var PrivateMessage in privateMessages)
            {
                _messageManager.MessageDeliveredAsync(PrivateMessage.Id).Wait();
                _hubContext.Clients.Group(PrivateMessage.PrivateChatId.ToString()).SendAsync("privateMessageStatusChanged", $"{PrivateMessage.Id} is Delivered", PrivateMessage).Wait();
            }

            //Pending Messages Delivered
        }

        public async Task OnDisconnectedAsync(Guid currentUserId, string connectionId)
        {
            string currentUserName = await _userRepo.GetUserName(currentUserId);
            List<PrivateChat> userPrivateChats = _privateChatService.GetAllUserPrivateChatsAsync(currentUserId).WaitAndGetResult().ToList();
            _userConnectionsManager.RemoveUserConnectionAsync(connectionId).Wait();
            List<Friendship> friends = _friendshipManager.GetFriendsAsync(currentUserId).WaitAndGetResult().ToList();


            //Remove From All private Chats Group
            foreach (var item in userPrivateChats)
            {
                _hubContext.Groups.RemoveFromGroupAsync(connectionId, item.Id.ToString());
            }

            //Remove From All Groups

            //Remove User From Online Group And Notify
            //TODO: Notify Only [Friends]
            bool stillOnline = _userConnectionsManager.IsOnline(currentUserId).WaitAndGetResult();
            foreach (var item in friends)
            {
                _hubContext.Groups.RemoveFromGroupAsync(connectionId, item.FriendshipId.ToString()).Wait();
                if (!stillOnline)
                {
                    _hubContext.Clients.Group(item.FriendshipId.ToString()).SendAsync("userstatus", currentUserName, "Offline");
                }
            }
        }
    }

    
}
