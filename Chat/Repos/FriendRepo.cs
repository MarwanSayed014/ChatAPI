using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;

namespace ChatAPI.Repos
{
    public class FriendRepo : Repo<Friend>, IFriendRepo
    {
        public FriendRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
