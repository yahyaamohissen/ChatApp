using Microsoft.EntityFrameworkCore;
using SignalR.Data.Models;

namespace SignalR.Data
{
    public class ChatAppDbContext : DbContext
    {
        //public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<GroupChatCategory> GroupChatCategories { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; } // to be added later


        public ChatAppDbContext(DbContextOptions<ChatAppDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<GroupChat>()
            //    .HasKey(cg => cg.Id);
            modelBuilder.Entity<User>()
                .HasKey(cg => cg.Id);

            modelBuilder.Entity<User>()
                .Property(user => user.UserName)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasIndex(user => user.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(user => user.Email)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder.Entity<GroupChatCategory>()
                .HasKey(cg => cg.Id);
            modelBuilder.Entity<Friendship>()
                .HasKey(f => new { f.UserId, f.FriendId });

            //modelBuilder.Entity<GroupChat>()
            //    .HasMany(e => e.Members)
            //    .WithMany(e => e.GroupsChats);

            //modelBuilder.Entity<GroupChat>()
            //    .HasMany(e => e.Categories);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany(u => u.FriendOf)
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configure the relationship between User and ChatMessage for the Sender
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between User and ChatMessage for the Receiver
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(cm => cm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);



            // for in memory DB and tests 
            modelBuilder.Entity<User>()
                .HasData(new User { Id = 1, Email = "admin@test.com", Name = "Admin", UserName = "admin", Password = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8" });
        }
    }
}