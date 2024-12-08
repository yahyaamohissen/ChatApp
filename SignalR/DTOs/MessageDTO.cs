using SignalR.Models;

namespace SignalR.DTOs
{
    public class MessageDTO
    {
        public string Content { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime SentAt { get; set; }

        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
    }
}
