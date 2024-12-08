using SignalR.Data.Models;
using SignalR.DTOs;
using SignalR.Models;

namespace SignalR.Contracts
{
    public interface IMessagesService
    {
        public Task<MessageDTO> SendMessagetToFriend(int userId, string message, string friendUsername);
        public Task<ChatMessage> SendMessageToGroup(string message);
        public Task<bool> ChangeMessageStatus(int messageId, MessageStatus newStatus);
        public Task<List<MessageDTO>> GetMessagesWithFriend(int userId, string friendUsername);
    }
}