using ChatAPI.Dtos;
using ChatAPI.Helpers;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;
using ChatAPI.Types;
using Microsoft.EntityFrameworkCore;
using System;

namespace ChatAPI.Services
{
    public class ChatService : IChatService
    {
        public IPrivateChatRepo _privateChatRepo { get; }
        public IUserPrivateChatRepo _userPrivateChatRepo { get; }
        public IPrivateMessageRepo _privateMessageRepo { get; }
        public IUserRepo _userRepo { get; }

        public ChatService(IPrivateChatRepo chatRepo, IUserPrivateChatRepo userPrivateChatRepo,
            IPrivateMessageRepo messageRepo, IUserRepo userRepo)
        {
            _privateChatRepo = chatRepo;
            _userPrivateChatRepo = userPrivateChatRepo;
            _privateMessageRepo = messageRepo;
            _userRepo = userRepo;
        }



        public async Task<(PrivateChat privateChat, PrivateMessage PrivateMessage)> PrivateMessageingAsync(Guid CurrentUserId, PrivateMessageDto messageDto)
        {
            if (CurrentUserId == messageDto.AnotherUserId) return (null,null);
            if (!await _userRepo.UserExists(CurrentUserId) || !await _userRepo.UserExists(messageDto.AnotherUserId))
                return (null, null);
            if(string.IsNullOrEmpty(messageDto.Text) && (messageDto.Files.Count() == 0 || messageDto.Files == null))
                return (null, null);

            PrivateChat privateChat = await _userPrivateChatRepo.GetPrivateChatAsync(CurrentUserId, messageDto.AnotherUserId);
            if (privateChat == null)
            {
                privateChat = new PrivateChat();
                await _privateChatRepo.CreateAsync(privateChat);
                await _privateChatRepo.SaveAsync();

                await _userPrivateChatRepo.AddUsersToPrivateChat(
                    new List<Guid>
                    {
                        CurrentUserId,
                        messageDto.AnotherUserId
                    },
                    privateChat.Id);
                await _userPrivateChatRepo.SaveAsync();
            }


            var privateMessage = new PrivateMessage
            {
                Text = messageDto.Text,
                PrivateChatId = privateChat.Id,
                Date = DateTime.Now,
                MessageStatus = MessageStatus.Sent,
                SenderId = CurrentUserId
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
                        //attachmentPaths.Add($"/Images/PrivateChatsAttachments/{imageName}");
                        privateMessage.AttachmentPaths += $"/Images/PrivateChatsAttachments/{imageName},";
                    }
                }
            }
            //privateMessage.AttachmentPaths = "sfsafas.png";

            await _privateMessageRepo.CreateAsync(privateMessage);
            await _privateMessageRepo.SaveAsync();
            return (privateChat, privateMessage);
        }

        public async Task<IEnumerable<PrivateChat>> GetAllUserPrivateChatsAsync(Guid userId)
        {
            return (await _userPrivateChatRepo.GetAllUserPrivateChatsAsync(userId)).ToList();
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
