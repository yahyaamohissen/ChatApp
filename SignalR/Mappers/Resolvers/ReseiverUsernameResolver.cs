using AutoMapper;
using SignalR.Contracts;
using SignalR.Data.Models;
using SignalR.DTOs;

namespace SignalR.Mappers.Resolvers
{
    public class ReseiverUsernameResolver : IValueResolver<ChatMessage, MessageDTO, string>
    {
        private readonly IUserService userService;

        public ReseiverUsernameResolver(IUserService userService)
        {
            this.userService = userService;
        }

        public string Resolve(ChatMessage source, MessageDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrWhiteSpace(source.Receiver?.UserName))
                return source.Receiver.UserName;

            var user = userService.GetUser(source.ReceiverId).Result;
            if (user == null)
                return "";

            return user.UserName;
        }
    }
}