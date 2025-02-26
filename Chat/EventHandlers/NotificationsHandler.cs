using ChatAPI.Dtos;
using ChatAPI.Hubs;
using ChatAPI.Models;
using ChatAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;


namespace ChatAPI.EventHandlers
{
    public class NotificationsHandler
    {
        public IUserConnectionsManager _userConnectionsManager { get; }
        public Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> _hubContext { get; set; }

        public NotificationsHandler(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, IUserConnectionsManager userConnectionsManager)
        {
            _userConnectionsManager = userConnectionsManager;
            _hubContext = hubContext;
        }

        public async Task NewPrivateMessageNotification(PrivateMessageNotificationDto dto) 
        {
            //add to private chat group
            var currentUserConnectionIds = await _userConnectionsManager.GetUserConnectionsId(dto.PrivateMessage.SenderId);
            foreach (var conId in currentUserConnectionIds)
            {
                await _hubContext.Groups.AddToGroupAsync(conId, dto.PrivateMessage.Id.ToString());
            }

            //Add other userIds To private chat group
            var anotherUserConnectionIds = await _userConnectionsManager.GetUserConnectionsId(dto.AnotherUserId);
            foreach (var conId in anotherUserConnectionIds)
            {
                await _hubContext.Groups.AddToGroupAsync(conId, dto.PrivateMessage.Id.ToString());
            }

            _hubContext.Clients.Group(dto.PrivateMessage.Id.ToString()).SendAsync("newPrivateMessage", dto.PrivateMessage.SenderId, dto.SenderName, dto.PrivateMessage);

        }

        public async Task PrivateMessageDeliveredNotification(PrivateMessage privateMessage)
        {
            _hubContext.Clients.Group(privateMessage.Id.ToString()).SendAsync("privateMessageStatusChanged", $"{privateMessage.Id} is Delivered", privateMessage);
        }
    }
}
