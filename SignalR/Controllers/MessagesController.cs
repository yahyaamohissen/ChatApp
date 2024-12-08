using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalR.Contracts;
using SignalR.DTOs;

namespace SignalR.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMessagesService messagesService;

        public MessagesController(IMessagesService messagesService)
        {
            this.messagesService = messagesService;
        }

        [HttpGet("api/{userId}/messages/friend/{friendUsername}/history")]
        public async Task<ActionResult<List<MessageDTO>>> GetFriendMessagesHistory(int userId, string friendUsername)
        {
            var list = await messagesService.GetMessagesWithFriend(userId, friendUsername);
            return Ok(list);
        }
    }
}