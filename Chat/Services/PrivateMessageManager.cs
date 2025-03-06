using ChatAPI.Dtos;
using ChatAPI.EventHandlers;
using ChatAPI.Helpers;
using ChatAPI.Hubs;
using ChatAPI.Models;
using ChatAPI.Repos;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;
using ChatAPI.Types;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Services
{
    public class PrivateMessageManager : IPrivateMessageManager
    {
        public IPrivateChatService _privateChatService { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }
        public IUserPrivateChatRepo _userPrivateChatRepo { get; }
        public IPrivateMessageRepo _privateMessageRepo { get; }
        public NotificationsHandler _notificationsHandler { get; }

        public event NewPreivateMessageEventHandler NewPrivateMessageEvent;
        public delegate Task NewPreivateMessageEventHandler(PrivateMessageNotificationDto dto);

        public event PreivateMessageDeliveredEventHandler PreivateMessageDeliveredEvent;
        public delegate Task PreivateMessageDeliveredEventHandler(PrivateMessage privateMessage);

        public IUserRepo _userRepo { get; }

        public PrivateMessageManager(IPrivateChatService privateChatService, IUserConnectionsManager userConnectionsManager,
            IUserRepo userRepo, IUserPrivateChatRepo userPrivateChatRepo, IPrivateMessageRepo privateMessageRepo,
            NotificationsHandler notificationsHandler)
        {
            _privateChatService = privateChatService;
            _userConnectionsManager = userConnectionsManager;
            _userRepo = userRepo;
            _userPrivateChatRepo = userPrivateChatRepo;
            _privateMessageRepo = privateMessageRepo;
            _notificationsHandler = notificationsHandler;
        }

        public NewPreivateMessageEventHandler GetNewPreivateMessage()
        {
            return NewPrivateMessageEvent;
        }

        public async Task PrivateMessageingAsync(Guid currentUserId, PrivateMessageDto messageDto)
        {
            string currentUserName = await _userRepo.GetUserName(currentUserId);
            if (currentUserId == messageDto.AnotherUserId) return;
            if (!await _userRepo.UserExists(currentUserId) || !await _userRepo.UserExists(messageDto.AnotherUserId))
                return;
            if (string.IsNullOrEmpty(messageDto.Text) && (messageDto.Files.Count() == 0 || messageDto.Files == null))
                return;

            PrivateChat privateChat = await _privateChatService.GetPrivateChatAsync(currentUserId, messageDto.AnotherUserId);
            if (privateChat == null)
            {
                privateChat = new PrivateChat();
                await _privateChatService.CreatePrivateChat(privateChat, currentUserId, messageDto.AnotherUserId);
            }

            var privateMessage = new PrivateMessage
            {
                Text = messageDto.Text,
                PrivateChatId = privateChat.Id,
                Date = DateTime.Now,
                MessageStatus = MessageStatus.Sent,
                SenderId = currentUserId
            };
            List<string> attachmentPaths = new List<string>();
            if (messageDto.Files != null)
            {
                foreach (var item in messageDto.Files)
                {
                    if (item != null)
                    {
                        //Upload attachment
                        var dir = Directory.GetCurrentDirectory() + "/wwwroot/Images/PrivateChatsAttachments/";
                        var ex = ServerFile.GetExtension(item.FileName);
                        var imageName = Guid.NewGuid() + ex;
                        var imagePath = dir + imageName;
                        ServerFile.Upload(item, imagePath);
                        privateMessage.AttachmentPaths += $"/Images/PrivateChatsAttachments/{imageName},";
                    }
                }
            }

            await _privateMessageRepo.CreateAsync(privateMessage);
            await _privateMessageRepo.SaveAsync();

            if (privateChat != null)
            {
                var privateMessageNotificationDto = new PrivateMessageNotificationDto 
                {
                    PrivateMessage = privateMessage,
                    AnotherUserId = messageDto.AnotherUserId,
                    SenderName = currentUserName
                };

                NewPrivateMessageEvent += _notificationsHandler.NewPrivateMessageNotification;

                NewPrivateMessageEvent?.Invoke(privateMessageNotificationDto);

                var anotherUserConnectionIds = await _userConnectionsManager.GetUserConnectionsId(messageDto.AnotherUserId);
                //Update message Status to delivered if another user is online
                if (anotherUserConnectionIds.Count() >= 1)
                {
                    await MessageDeliveredAsync(privateMessage.Id);
                    PreivateMessageDeliveredEvent += _notificationsHandler.PrivateMessageDeliveredNotification;
                    PreivateMessageDeliveredEvent?.Invoke(privateMessage);
                }

                //TODO: message Status to read 
            }
        }

       
        public async Task MessageDeliveredAsync(Guid privateMessageId)
        {
            var message = (await _privateMessageRepo.FindAsync(x => x.Id == privateMessageId))
                .SingleOrDefault();
            if (message != null)
            {
                message.MessageStatus = MessageStatus.Delivered;
                await _privateMessageRepo.UpdateAsync(message);
                await _userPrivateChatRepo.SaveAsync();
            }
        }

        public async Task<IEnumerable<PrivateMessage>> GetPendingPrivateMessagesAsync(Guid userId)
        {
            List<PrivateMessage> PendingMessages = (await _privateMessageRepo.FindAsync(x => x.MessageStatus == MessageStatus.Sent
                                        && x.SenderId != userId)).ToList();
            return PendingMessages;
        }
    }
}
