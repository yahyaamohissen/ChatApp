using SignalR.Data.Models;

namespace SignalR.Contracts
{
    public interface IGroupManagementService
    {
        public Task<GroupChat> CreateGroupChat();
        public Task<GroupChat> DeleteGroupChat();
        public Task<GroupChat> GetGroupChat();
        public Task<List<GroupChat>> GetUserGroupChats(int userId);
        public Task<bool> AddUserToGroupChat();
        public Task<bool> DeleteUserFromGroupChat();
    }
}