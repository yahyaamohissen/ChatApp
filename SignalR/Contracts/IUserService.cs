using SignalR.Data.Models;

namespace SignalR.Contracts
{
    public interface IUserService
    {
        public Task<User> GetUserByUsername(string userName);
        public Task<User> GetUserByEmail(string email);
        public Task<User> GetUser(int id);
    }
}