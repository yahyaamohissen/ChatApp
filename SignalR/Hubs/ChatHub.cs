using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Contracts;
using SignalR.Data;
using SignalR.DTOs;
using SignalR.Models;
using System.Collections.Concurrent;

namespace SignalR.Hubs
{
    [Authorize]
    public class ChatHub: Hub
    {
        private readonly ChatAppDbContext dbContext;
        private readonly IMessagesService messagesService;
        private readonly IUserService userService;
        private readonly IGroupManagementService groupManagementService;

        private static ConcurrentDictionary<int, string> connectedUsers = new ConcurrentDictionary<int, string>();

        public ChatHub(ChatAppDbContext dbContext,
            IMessagesService messagesService,
            IUserService userService)//,
            //IGroupManagementService groupManagementService)
        {
            this.dbContext = dbContext;
            this.messagesService = messagesService;
            this.userService = userService;
            //this.groupManagementService = groupManagementService;
        }

        public override async Task OnConnectedAsync()
        {
            // Get the user identifier
            var userId = Context.UserIdentifier;

            // Or access user claims
            var userName = Context.User?.Identity?.Name;

            int.TryParse(userId, out int convertedUserId);


            connectedUsers.TryAdd(convertedUserId, Context.ConnectionId);

            //var userGroups = await groupManagementService.GetUserGroupChats(convertedUserId);
            //if (userGroups?.Any() == true)
            //{
            //    foreach (var group in userGroups)
            //    {
            //        await Groups.AddToGroupAsync(Context.ConnectionId, $"{group.Name}-{group.Id}");
            //    }
            //}
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            // Or access user claims
            var userName = Context.User?.Identity?.Name;

            int.TryParse(userId, out int convertedUserId);
            connectedUsers.TryRemove(convertedUserId, out var value);

            //var userGroups = await groupManagementService.GetUserGroupChats(convertedUserId);
            //if (userGroups?.Any() == true)
            //{
            //    foreach (var group in userGroups)
            //    {
            //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{group.Name}-{group.Id}");
            //    }
            //}

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToFriend(string friendUsername, string message)
        {
            int.TryParse(Context.UserIdentifier, out int userId); // sender 

            var friend = await userService.GetUserByUsername(friendUsername);

            var sentMessage = await messagesService.SendMessagetToFriend(userId, message, friendUsername);


            if (connectedUsers.TryGetValue(friend.Id, out var connectionId))
            {
                //send to a client
                await Clients.Client(connectionId).SendAsync("ReceiveMessageFromFriend", sentMessage);
            }
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessageFromFriend", sentMessage);
        }

        public async Task SetMessageStatus(int messageId, MessageStatus status) //to be moved as a rest api
        {
            var result = await messagesService.ChangeMessageStatus(messageId, status);
            if (result)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ChangeMessageStatus", messageId, status);
            }
        }

        public async Task SendMessageToGroup(string groupId, string user, string message)
        {
            int.TryParse(Context.UserIdentifier, out var userId);
            var group = await groupManagementService.GetGroupChat();
            var sentMessage = await messagesService.SendMessageToGroup(message);

            if (sentMessage != null)
            {
                await Clients.Groups($"{group.Name}-{group.Id}").SendAsync("ReceiveMessageFromGroup", group.Id, group.Name, sentMessage);
            }
        }

        public async Task AddToGroup(string groupName)
        {
            int.TryParse(Context.UserIdentifier, out var userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ShowMessage", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ShowMessage", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}