using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;

namespace ChatAPI.Repos
{
    public class UserConnectionsRepo : Repo<UserConnection>, IUserConnectionsRepo
    {
        public UserConnectionsRepo(ApplicationDbContext context) : base(context)
        {
        }

        public async Task RemoveAllConnectionsAsync()
        {
            await DeleteAllAsync();
        }
    }
}
