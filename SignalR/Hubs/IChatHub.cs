using SignalR.DTOs;

namespace SignalR.Hubs
{
    public interface IChatHub
    {
        public Task<MessageDTO> SendMessageToFriend(string friendUserName, string message);
    }
}