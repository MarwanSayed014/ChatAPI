using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;

namespace ChatAPI.Repos
{
    public class GroupMessageStatusRepo : Repo<GroupMessageStatus>, IGroupMessageStatusRepo
    {
        public GroupMessageStatusRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
