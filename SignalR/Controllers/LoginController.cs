using Microsoft.AspNetCore.Mvc;
using SignalR.Contracts;
using SignalR.Helpers;
using SignalR.Models.HttpRequests;
using SignalR.Providers;
using Microsoft.AspNetCore.Authorization;

namespace SignalR.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly TokenProvider tokenProvider;
        private readonly IUserService userService;

        public LoginController(TokenProvider tokenProvider,
            IUserService userService)
        {
            this.tokenProvider = tokenProvider;
            this.userService = userService;
        }

        [HttpPost("api/auth/login")]
        public async Task<ActionResult<string>> login([FromBody]LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email should not be empty!");
            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Password should not be empty!");

            var user = await userService.GetUserByEmail(request.Email);
            if (user == null)
                return NotFound("User is not found!");

            var isPasswordMatch = PasswordHelper.VerifyPasswords(request.Password, user.Password);
            if (!isPasswordMatch)
                return Forbid("Password is incorrect!");

            var token = tokenProvider.Create(user);
            return token;
        }
    }
}