using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using System.Collections.Generic;

namespace ChatAPI.Repos
{
    public class UserRepo : Repo<User>, IUserRepo
    {
        public UserRepo(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> UserExists(Guid userId)
        {
            return (await FindAsync(x=> x.Id == userId)).SingleOrDefault() == null ? false : true;
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            if (userName == null)
                throw new NullReferenceException("UserName should not be null");
            return (await FindAsync(x => x.Name == userName)).ToList().Count() > 0 ? true : false;
        }
    }
}
