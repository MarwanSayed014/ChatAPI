using ChatAPI.Dtos;
using ChatAPI.Hubs;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services
{
    public class MessageManager : IMessageManager
    {
        public IChatService _chatService { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }
        public IUserRepo _userRepo { get; }

        public MessageManager(IChatService chatService, IUserConnectionsManager userConnectionsManager,
            IUserRepo userRepo)
        {
            _chatService = chatService;
            _userConnectionsManager = userConnectionsManager;
            _userRepo = userRepo;
        }
        public async Task PrivateMessageingAsync(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext,
                                                Guid currentUserId, PrivateMessageDto messageDto)
        {
            string currentUserName = await _userRepo.GetUserName(currentUserId);
            (PrivateChat privateChat, PrivateMessage privateMessage) = await _chatService.PrivateMessageingAsync(currentUserId, messageDto);
            if (privateChat != null)
            {
                //add to private chat group
                var currentUserConnectionIds = await _userConnectionsManager.GetUserConnectionsId(currentUserId);
                foreach (var conId in currentUserConnectionIds)
                {
                    await hubContext.Groups.AddToGroupAsync(conId, privateChat.Id.ToString());
                }

                //Add other userIds To private chat group
                var anotherUserConnectionIds = await _userConnectionsManager.GetUserConnectionsId(messageDto.AnotherUserId);
                foreach (var conId in anotherUserConnectionIds)
                {
                    await hubContext.Groups.AddToGroupAsync(conId, privateChat.Id.ToString());
                }



                hubContext.Clients.Group(privateChat.Id.ToString()).SendAsync("newPrivateMessage", currentUserId, currentUserName, privateMessage);

                //Update message Status to delivered if another user is online
                if (anotherUserConnectionIds.Count() >= 1)
                {
                    await _chatService.MessageDeliveredAsync(privateMessage.Id);

                    hubContext.Clients.Group(privateChat.Id.ToString()).SendAsync("privateMessageStatusChanged", $"{privateMessage.Id} is Delivered", privateMessage);
                }

                //TODO: message Status to read 
            }
        }
    }
}
