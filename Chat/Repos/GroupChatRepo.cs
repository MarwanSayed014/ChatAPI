using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;

namespace ChatAPI.Repos
{
    public class GroupChatRepo : Repo<GroupChat> , IGroupChatRepo
    {
        public GroupChatRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
