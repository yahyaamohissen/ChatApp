using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SignalR.Data.Models;
using System.Security.Claims;
using System.Text;

namespace SignalR.Providers
{
    public sealed class TokenProvider
    {
        private readonly IConfiguration configuration;

        public TokenProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Create(User user)
        {
            var secretKey = configuration["Security:JWT:Secret"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> { 
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("username", user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.Name)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(configuration["Security:JWT:ExpirationInMinutes"])), // or from config
                SigningCredentials = credentials,
                Issuer = configuration["Security:JWT:Issuer"], //to see what is it? config
                Audience = configuration["Security:JWT:Audience"], // what is it? config
            };

            var tokenHandler = new JsonWebTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}
