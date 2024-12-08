using Microsoft.EntityFrameworkCore;
using SignalR.Contracts;
using SignalR.Data;
using SignalR.Data.Models;

namespace SignalR.Services
{
    public class UserService : IUserService
    {
        private readonly ChatAppDbContext dbContext;

        public UserService(ChatAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<User> GetUserByUsername(string userName)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.UserName == userName);
            return user;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
            return user;
        }
    }
}
