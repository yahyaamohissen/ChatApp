namespace SignalR.Data.Models
{
    public class GroupChat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<GroupChatCategory> Categories { get; set; }
        public ICollection<User> Members { get; set; }
        public ICollection<ChatMessage> SentMessages { get; set; }
    }
}