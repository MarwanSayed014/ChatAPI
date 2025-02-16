using ChatAPI.Models;

namespace ChatAPI.Repos.Interfaces
{
    public interface IFriendshipRepo : IRepo<Friendship>
    {
        public Task<IEnumerable<Friendship>> GetFriendsAsync(Guid userId);
        public Task<IEnumerable<Friendship>> GetPendingFriendRequestsAsync(Guid userId);
        public Task<IEnumerable<Friendship>> GetRecentlyAcceptedRequestAsync(Guid userId);
    }
}
