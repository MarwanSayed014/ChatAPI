using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;

namespace ChatAPI.Repos
{
    public class PrivateChatRepo : Repo<PrivateChat>, IPrivateChatRepo
    {
        public PrivateChatRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
