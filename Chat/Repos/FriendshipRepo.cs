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
        public async Task<IEnumerable<Friendship>> GetFriendsAsync(Guid userId)
        {
            return (await FindAsync(x =>
                        (x.RequestorUserId == userId || x.RespondentUserId == userId)
                            && x.FriendshipStatus == FriendshipStatus.Friends)
                    );
        }
        public async Task<IEnumerable<Friendship>> GetPendingFriendRequestsAsync(Guid userId)
        {
            return (await FindAsync(x =>
                        x.RespondentUserId == userId
                            && x.IsDelivered == false)
                    );
        }

        public async Task<IEnumerable<Friendship>> GetRecentlyAcceptedRequestAsync(Guid userId)
        {
            return (await FindAsync(x =>
                        x.RequestorUserId == userId
                            && x.NotifyAcceptance == true)
                    );
        }
    }
}
