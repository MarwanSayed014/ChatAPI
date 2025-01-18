using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;

namespace ChatAPI.Repos
{
    public class UserGroupChatRepo : Repo<UserGroupChat>, IUserGroupChatRepo
    {
        public UserGroupChatRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
