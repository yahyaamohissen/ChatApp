namespace SignalR.Data.Models
{
    public class Friendship
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int FriendId { get; set; }
        public User Friend { get; set; }

        
        public DateTime FriendshipDate { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsActive { get; set; }
    }
}