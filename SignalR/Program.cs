using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SignalR.Contracts;
using SignalR.Data;
using SignalR.Data.Models;
using SignalR.Hubs;
using SignalR.Mappers;
using SignalR.Providers;
using SignalR.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add the "Authorization" input to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddDbContext<ChatAppDbContext>(options =>
{
    options.UseInMemoryDatabase("ChatApp");
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DepotAllocationDB"));
});

builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddAutoMapper(typeof(ChattingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
                      policyBuilder =>
                      {
                          policyBuilder
                          .WithOrigins("http://localhost:3000")
                          //.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                      });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Security:JWT:Secret"])),//from config
            ValidIssuer = builder.Configuration["Security:JWT:Issuer"],
            ValidAudience = builder.Configuration["Security:JWT:Audience"],
            ClockSkew = TimeSpan.Zero,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"]; //context.Request.Headers.Authorization.ToString();

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/chatHub")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSignalR(options => {
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);  // Time to wait for a keep-alive packet from the client
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);      // Interval for the server to send keep-alive packets
    options.EnableDetailedErrors = true;
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatAppDbContext>();
    dbContext.Users.Add(
            new User { 
                Id = 1, 
                Email = "admin@test.com", 
                Name = "Admin", 
                UserName = "admin",
                Password = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8" 
            }
        );
    dbContext.Users.Add(
            new User { 
                Id = 2, 
                Email = "admin2@test.com", 
                Name = "Admin2", 
                UserName = "admin2",
                Password = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8" 
            }
        );
    dbContext.Friendships.Add(new Friendship
    {
        UserId = 1,
        FriendId = 2,
        IsActive = true,
        IsFavorite = true,
        FriendshipDate = DateTime.Today
    });
    dbContext.Friendships.Add(new Friendship
    {
        UserId = 2,
        FriendId = 1,
        IsActive = true,
        IsFavorite = true,
        FriendshipDate = DateTime.Today
    });

    dbContext.ChatMessages.Add(new ChatMessage {
        Content = "Hi",
        Id = 1,
        ReceivedAt = DateTime.Now.AddMinutes(-4),
        SentAt = DateTime.Now.AddMinutes(-6),
        SenderId = 1,
         ReceiverId = 2,
    });
    dbContext.ChatMessages.Add(new ChatMessage
    {
        Content = "Hllo friend",
        Id = 2,
        ReceivedAt = DateTime.Now.AddMinutes(-2),
        SentAt = DateTime.Now.AddMinutes(-5),
        SenderId = 2,
        ReceiverId = 1,
    });

    await dbContext.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//    endpoints.MapHub<ChatHub>("/chatHub"); // Register the Hub
//});

app.MapControllers();

app.Run();
