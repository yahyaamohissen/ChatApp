using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SignalR.Contracts;
using SignalR.Data;
using SignalR.Data.Models;
using SignalR.DTOs;
using SignalR.Models;

namespace SignalR.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly ChatAppDbContext dbContext;
        private readonly IMapper mapper;

        public MessagesService(ChatAppDbContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<bool> ChangeMessageStatus(int messageId, MessageStatus newStatus)
        {
            var message = dbContext.ChatMessages.Single(message => message.Id == messageId);
            if (message == null)
                return false;

            message.Status = newStatus;
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<ChatMessage> SendMessageToGroup(string message)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageDTO> SendMessagetToFriend(int userId, string message, string friendUsername)
        {
            var friend = dbContext.Users.FirstOrDefault(user => user.UserName == friendUsername);
            if (friend == null)
                return null;

            var user = dbContext.Users.Single(user => user.Id == userId);
            var sentMessage = new ChatMessage
            {
                Content = message,
                SenderId = userId,
                ReceiverId = friend.Id,
                SentAt = DateTime.UtcNow,
                Status = MessageStatus.Sent
            };

            dbContext.ChatMessages.Add(sentMessage);

            await dbContext.SaveChangesAsync();

            return mapper.Map<MessageDTO>(sentMessage);
        }

        public async Task<List<MessageDTO>> GetMessagesWithFriend(int userId, string friendUsername)
        {
            var friend = await dbContext.Users.FirstOrDefaultAsync(user => user.UserName == friendUsername);
            var user = await dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
            //var areFriends = friend.Friends.Any(friend => friend.FriendId == userId) && // for later to check on friendship
            //    friend.FriendOf.Any(friend => friend.FriendId == userId);
            if (friend == null || user == null)
                return new List<MessageDTO>();

            var ids = new List<int> { userId, friend.Id };

            var allMessages = await dbContext.ChatMessages
                .Where(message => ids.Contains(message.SenderId) && ids.Contains(message.ReceiverId))
                .OrderBy(message => message.SentAt)
                .ToListAsync();

            return mapper.Map<List<MessageDTO>>(allMessages);
        }
    }
}