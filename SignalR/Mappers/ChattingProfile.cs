using AutoMapper;
using SignalR.Data.Models;
using SignalR.DTOs;
using SignalR.Mappers.Resolvers;

namespace SignalR.Mappers
{
    public class ChattingProfile: Profile
    {
        public ChattingProfile()
        {
            CreateMap<ChatMessage, MessageDTO>()
                .ForMember(d => d.ReceiverUsername, o => o.MapFrom<ReseiverUsernameResolver>())
                .ForMember(d => d.SenderUsername, o => o.MapFrom<SenderUsernameResolver>());
        }
    }
}