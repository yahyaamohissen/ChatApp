using SignalR.Models;

namespace SignalR.Data.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime SentAt { get; set; }

        // Foreign keys
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        // Navigation properties
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
}