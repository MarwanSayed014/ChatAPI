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
    public class PrivateChatService : IPrivateChatService
    {
        public IPrivateChatRepo _privateChatRepo { get; }
        public IUserPrivateChatRepo _userPrivateChatRepo { get; }
        public IPrivateMessageRepo _privateMessageRepo { get; }
        public IUserRepo _userRepo { get; }

        public PrivateChatService(IPrivateChatRepo chatRepo, IUserPrivateChatRepo userPrivateChatRepo,
            IPrivateMessageRepo messageRepo, IUserRepo userRepo)
        {
            _privateChatRepo = chatRepo;
            _userPrivateChatRepo = userPrivateChatRepo;
            _privateMessageRepo = messageRepo;
            _userRepo = userRepo;
        }



        public async Task<IEnumerable<PrivateChat>> GetAllUserPrivateChatsAsync(Guid userId)
        {
            return (await _userPrivateChatRepo.GetAllUserPrivateChatsAsync(userId)).ToList();
        }

        public async Task<PrivateChat> GetPrivateChatAsync(Guid currentUserId, Guid anotherUserId)
        {
            return await _userPrivateChatRepo.GetPrivateChatAsync(currentUserId, anotherUserId);
        }

        public async Task CreatePrivateChat(PrivateChat privateChat, Guid currentUserId, Guid anotherUserId)
        {
            await _privateChatRepo.CreateAsync(privateChat);
            await _privateChatRepo.SaveAsync();

            await _userPrivateChatRepo.AddUsersToPrivateChat(
                new List<Guid>
                {
                        currentUserId,
                        anotherUserId
                },
                privateChat.Id);
            await _userPrivateChatRepo.SaveAsync();
        }
    }
}
