using ChatAPI.Models;

namespace ChatAPI.Services.Interfaces
{
    public interface IUserConnectionsManager
    {
        Task<IEnumerable<string>> GetUserConnectionsId(Guid userId);
        Task CreateUserConnectionAsync(UserConnection userConnection);
        Task RemoveUserConnectionAsync(string signalRConnectionId);
        Task RemoveAllConnectionsAsync();
        Task<bool> IsOnline(Guid userId);

    }
}
