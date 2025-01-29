using ChatAPI.Dtos;
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
        public IChatService _chatService { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }
        public IUserRepo _userRepo { get; }

        public ChatHubManager(IChatService chatService, IUserConnectionsManager userConnectionsManager,
            IUserRepo userRepo)
        {
            _chatService = chatService;
            _userConnectionsManager = userConnectionsManager;
            _userRepo = userRepo;
        }
        public async Task OnConnectedAsync(IHubCallerClients clients, IGroupManager groups, 
            Guid currentUserId, string connectionId)
        {
            string currentUserName = await _userRepo.GetUserName(currentUserId);
            List<PrivateChat> userPrivateChats = _chatService.GetAllUserPrivateChatsAsync(currentUserId).WaitAndGetResult().ToList();
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
                groups.AddToGroupAsync(connectionId, item.Id.ToString()).Wait();
            }

            //add to all groups



            //Create Online Group And Add User To It And Notify
            //TODO: Notify Only [Friends]
            if (!wasOnline)
            {
                clients.AllExcept(connectionId).SendAsync("userstatus", currentUserName, "Online");
            }

            //Push Pending Private Messages
            List<PrivateMessage> privateMessages = _chatService.GetPendingPrivateMessagesAsync(currentUserId).WaitAndGetResult().ToList();
            foreach (var PrivateMessage in privateMessages)
            {
                _chatService.MessageDeliveredAsync(PrivateMessage.Id).Wait();
                clients.Group(PrivateMessage.PrivateChatId.ToString()).SendAsync("privateMessageStatusChanged", $"{PrivateMessage.Id} is Delivered", PrivateMessage);
            }

            //Pending Messages Delivered
        }

        public async Task OnDisconnectedAsync(IHubCallerClients clients, IGroupManager groups, Guid currentUserId, string connectionId)
        {
            string currentUserName = await _userRepo.GetUserName(currentUserId);
            List<PrivateChat> userPrivateChats = _chatService.GetAllUserPrivateChatsAsync(currentUserId).WaitAndGetResult().ToList();
            _userConnectionsManager.RemoveUserConnectionAsync(connectionId).Wait();

            //Remove From All private Chats Group
            foreach (var item in userPrivateChats)
            {
                groups.RemoveFromGroupAsync(connectionId, item.Id.ToString());
            }

            //Remove From All Groups

            //Remove User From Online Group And Notify
            //TODO: Notify Only [Friends]
            bool stillOnline = _userConnectionsManager.IsOnline(currentUserId).WaitAndGetResult();
            if (!stillOnline)
            {
                clients.All.SendAsync("userstatus", currentUserName, "Offline");
            }
        }
    }

    
}
