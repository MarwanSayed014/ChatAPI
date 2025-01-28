using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Types;

namespace ChatAPI.Repos
{
    public class FriendshipRepo : Repo<Friendship>, IFriendshipRepo
    {
        public FriendshipRepo(ApplicationDbContext context) : base(context)
        {
        }


    }
}
