using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;

namespace ChatAPI.Services
{
    public class UserConnectionsManager : IUserConnectionsManager
    {
        public IUserConnectionsRepo _userConnectionsRepo { get; }

        public UserConnectionsManager(IUserConnectionsRepo userConnectionsRepo)
        {
            _userConnectionsRepo = userConnectionsRepo;
        }


        public async Task CreateUserConnectionAsync(UserConnection userConnection)
        {
            if (userConnection != null)
            {
                await _userConnectionsRepo.CreateAsync(userConnection);
                await _userConnectionsRepo.SaveAsync();
            }
        }

        public async Task RemoveUserConnectionAsync(string signalRConnectionId)
        {
            if (! string.IsNullOrEmpty( signalRConnectionId)) 
            {
                var userConnection = (await _userConnectionsRepo.FindAsync(x=> x.SignalRConnectionId == signalRConnectionId))
                    .SingleOrDefault();
                if (userConnection != null) 
                {
                    await _userConnectionsRepo.DeleteAsync(userConnection);
                    await _userConnectionsRepo.SaveAsync();
                }
            }
        }

        public async Task<IEnumerable<string>> GetUserConnectionsId(Guid userId)
        {
            return (await _userConnectionsRepo.FindAsync(x=> x.UserId == userId)).
                Select(x=> x.SignalRConnectionId).ToList();
        }

        public async Task RemoveAllConnectionsAsync()
        {
            await _userConnectionsRepo.RemoveAllConnectionsAsync();
            await _userConnectionsRepo.SaveAsync();
        }

        public async Task<bool> IsOnline(Guid userId)
        {
            return (await _userConnectionsRepo.FindAsync(x => x.UserId == userId)).ToList().Count() >= 1 ? true : false;
        }
    }
}
