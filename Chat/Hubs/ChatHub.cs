using ChatAPI.Dtos;
using ChatAPI.Models;
using ChatAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;
using XAct;

namespace ChatAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private IChatService _chatService { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }

        public ChatHub(IChatService chatService, IUserConnectionsManager userConnectionsManager)
        {
            //RemoveAllUserConnections
            _chatService = chatService;
            _userConnectionsManager = userConnectionsManager;
        }
        [HubMethodName("privatechat")]
        //take Dto not parameters
        public async Task PrivateChat(PrivateMessageDto messageDto)
        {
            Guid currentUserId = new Guid();
            bool result = Guid.TryParse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value, out currentUserId);
            if (result == true) 
            {
                var currentUserName = Context.User.FindFirst(ClaimTypes.Name).Value;
                (PrivateChat privateChat, PrivateMessage PrivateMessage) = await _chatService.PrivateMessageingAsync(currentUserId, messageDto);
                if (privateChat != null) 
                {
                    //add to private chat group
                    var currentUserConnectionIds = await _userConnectionsManager.GetUserConnectionsId(currentUserId);
                    foreach (var conId in currentUserConnectionIds)
                    {
                        await Groups.AddToGroupAsync(conId, privateChat.Id.ToString());
                    }

                    //Add other userIds To private chat group
                    var anotherUserConnectionIds = await _userConnectionsManager.GetUserConnectionsId(messageDto.AnotherUserId);
                    foreach (var conId in anotherUserConnectionIds)
                    {
                        await Groups.AddToGroupAsync(conId, privateChat.Id.ToString());
                    }

                    //Broadcasting message to all anpther user
                    Clients.Group(privateChat.Id.ToString()).SendAsync("newPrivateMessage", currentUserId, currentUserName, PrivateMessage);

                    //Update message Status to delivered if another user is online
                    if(anotherUserConnectionIds.Count() >= 1)
                    {
                        await _chatService.MessageDeliveredAsync(PrivateMessage.Id);
                        Clients.Group(privateChat.Id.ToString()).SendAsync("privateMessageStatusChanged", $"{PrivateMessage.Id} is Delivered", PrivateMessage);
                    }

                    //TODO: message Status to read 
                }

            }
            
        }

        [HubMethodName("groupchat")]
        public async Task GroupChat(Guid groupid)
        {
            //Save in DB


            //Broadcasting to all online clients
            Clients.All.SendAsync("newwmessage", "Test");

        }


        public override Task OnConnectedAsync()
        {
            Guid currentUserId = new Guid();
            bool result = Guid.TryParse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value, out currentUserId);
            string conId = Context.ConnectionId;

            if (result == true  && conId != null) 
            {
                var currentUserName = Context.User.FindFirst(ClaimTypes.Name).Value;
                List<PrivateChat> userPrivateChats =  _chatService.GetAllUserPrivateChatsAsync(currentUserId).WaitAndGetResult().ToList();
                bool wasOnline = _userConnectionsManager.IsOnline(currentUserId).WaitAndGetResult();
                

                //Add To Connection Table
                _userConnectionsManager.CreateUserConnectionAsync(new UserConnection
                {
                    SignalRConnectionId = conId,
                    UserId = currentUserId
                }).Wait();

                //Add To All Private Chats
                foreach (var item in userPrivateChats)
                {
                    Groups.AddToGroupAsync(conId, item.Id.ToString()).Wait();
                }

                //add to all groups



                //Create Online Group And Add User To It And Notify
                //TODO: Notify Only [Friends]
                if (!wasOnline) 
                {
                    Clients.AllExcept(conId).SendAsync("userstatus", currentUserName, "Online");
                }

                //Push Pending Private Messages
                List<PrivateMessage> privateMessages = _chatService.GetPendingPrivateMessagesAsync(currentUserId).WaitAndGetResult().ToList();
                foreach (var PrivateMessage in privateMessages)
                {
                    _chatService.MessageDeliveredAsync(PrivateMessage.Id).Wait();
                    Clients.Group(PrivateMessage.PrivateChatId.ToString()).SendAsync("privateMessageStatusChanged", $"{PrivateMessage.Id} is Delivered", PrivateMessage);
                }
                
                //Pending Messages Delivered
            }



            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Guid currentUserId = new Guid();
            bool result = Guid.TryParse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value, out currentUserId);
            string conId = Context.ConnectionId;

            if (result == true && conId != null)
            {
                var currentUserName = Context.User.FindFirst(ClaimTypes.Name).Value;
                List<PrivateChat> userPrivateChats = _chatService.GetAllUserPrivateChatsAsync(currentUserId).WaitAndGetResult().ToList();
                _userConnectionsManager.RemoveUserConnectionAsync(conId).Wait();

                //Remove From All private Chats Group
                foreach (var item in userPrivateChats)
                {
                    Groups.RemoveFromGroupAsync(conId, item.Id.ToString());
                }

                //Remove From All Groups

                //Remove User From Online Group And Notify
                //TODO: Notify Only [Friends]
                bool stillOnline = _userConnectionsManager.IsOnline(currentUserId).WaitAndGetResult();
                if (!stillOnline)
                {
                    Clients.All.SendAsync("userstatus", currentUserName, "Offline");
                }

            }
            return base.OnDisconnectedAsync(exception);
        }

    }

}
