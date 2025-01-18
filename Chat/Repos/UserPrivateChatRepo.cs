using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Types;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Repos
{
    public class UserPrivateChatRepo : Repo<UserPrivateChat>, IUserPrivateChatRepo
    {
        public UserPrivateChatRepo(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddUsersToPrivateChat(IEnumerable<Guid> usersIds, Guid chatId)
        {
            foreach (var id in usersIds)
            {
                await CreateAsync(new UserPrivateChat
                {
                    PrivateChatId = chatId,
                    UserId = id
                });
            }
        }

        public async Task<IEnumerable<PrivateChat>> GetAllUserPrivateChatsAsync(Guid userId)
        {
            return (await FindAsync(x=>x.UserId == userId)).Include(x=>x.PrivateChat).Select(x=>x.PrivateChat);
        }

        public async Task<PrivateChat> GetPrivateChatAsync(Guid currentUserId, Guid anotherUserId)
        {
            var currentUserPrivateChats = (await FindAsync(x => x.UserId == currentUserId)).
                Include(x => x.PrivateChat).Select(x => x.PrivateChat);

            var anotherUserPrivateChats = (await FindAsync(x => x.UserId == anotherUserId)).
                Include(x => x.PrivateChat).Select(x => x.PrivateChat);

            if (currentUserPrivateChats.Count() == 0 && anotherUserPrivateChats.Count() == 0) return null;

            return currentUserPrivateChats.Intersect(anotherUserPrivateChats).
                SingleOrDefault();
        }
    }
}
