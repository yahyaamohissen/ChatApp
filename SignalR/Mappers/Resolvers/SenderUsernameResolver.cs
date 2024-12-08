using AutoMapper;
using SignalR.Contracts;
using SignalR.Data;
using SignalR.Data.Models;
using SignalR.DTOs;

namespace SignalR.Mappers.Resolvers
{
    public class SenderUsernameResolver : IValueResolver<ChatMessage, MessageDTO, string>
    {
        private readonly IUserService userService;

        public SenderUsernameResolver(IUserService userService)
        {
            this.userService = userService;
        }

        public string Resolve(ChatMessage source, MessageDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrWhiteSpace(source.Sender?.UserName))
                return source.Sender.UserName;

            var user = userService.GetUser(source.SenderId).Result;
            if (user == null)
                return "";

            return user.UserName;
        }
    }
}