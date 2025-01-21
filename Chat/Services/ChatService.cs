using ChatAPI.Dtos;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;
using ChatAPI.Types;
using Microsoft.EntityFrameworkCore;

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


            //TODO : Upload attachments
            var privateMessage = new PrivateMessage
            {
                Text = messageDto.Text,
                PrivateChatId = privateChat.Id,
                Date = DateTime.Now,
                MessageStatus = MessageStatus.Sent
            };
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
            List<PrivateChat> privateChats = (await _userPrivateChatRepo.GetPrivateChatAsync(userId)).ToList();
            List<PrivateMessage> PendingMessages = new List<PrivateMessage>();
            foreach (var chat in privateChats)
            {
                PendingMessages.AddRange(
                    (await _privateMessageRepo.FindAsync(x=>x.PrivateChatId == chat.Id
                    && x.MessageStatus == MessageStatus.Sent))
                    .ToList()
                    );
            }
            return PendingMessages;
        }
    }
}
