namespace SignalR.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public ICollection<GroupChat> GroupsChats { get; set; }
        public virtual ICollection<Friendship> Friends { get; set; }
        public virtual ICollection<Friendship> FriendOf { get; set; }
        public virtual ICollection<ChatMessage> SentMessages { get; set; }
        public virtual ICollection<ChatMessage> ReceivedMessages { get; set; }
    }
}