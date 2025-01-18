using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;

namespace ChatAPI.Repos
{
    public class GroupMessageRepo : Repo<GroupMessage> , IGroupMessageRepo
    {
        public GroupMessageRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
