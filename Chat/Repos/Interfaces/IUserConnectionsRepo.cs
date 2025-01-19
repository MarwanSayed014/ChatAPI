using ChatAPI.Models;

namespace ChatAPI.Repos.Interfaces
{
    public interface IUserConnectionsRepo : IRepo<UserConnection>
    {
        Task RemoveAllConnectionsAsync();
    }
}
