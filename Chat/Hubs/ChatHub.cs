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
        public IChatHubManager _chatHubManager { get; }

        public ChatHub(IChatHubManager chatHubManager)
        {
            _chatHubManager = chatHubManager;
        }
        [HubMethodName("privatechat")]
        //take Dto not parameters
        public async Task PrivateChat(PrivateMessageDto messageDto)
        {
            Guid currentUserId = new Guid();
            bool result = Guid.TryParse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value, out currentUserId);
            if (result == true) 
            {
                _chatHubManager.PrivateMessageingAsync(Clients,Groups, currentUserId, messageDto).Wait(); 

            }

        }
        public override Task OnConnectedAsync()
        {
            Guid currentUserId = new Guid();
            bool result = Guid.TryParse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value, out currentUserId);
            string conId = Context.ConnectionId;

            if (result == true  && conId != null) 
            {
                _chatHubManager.OnConnectedAsync(Clients, Groups, currentUserId, conId).Wait();
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
                _chatHubManager.OnDisconnectedAsync(Clients, Groups, currentUserId, conId).Wait();
            }
            return base.OnDisconnectedAsync(exception);
        }

    }

}
