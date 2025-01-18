using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;

namespace ChatAPI.Repos
{
    public class PrivateMessageRepo : Repo<PrivateMessage>, IPrivateMessageRepo
    {
        public PrivateMessageRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
