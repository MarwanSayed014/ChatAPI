
using ChatAPI.Data;
using ChatAPI.EventHandlers;
using ChatAPI.Helpers;
using ChatAPI.Helpers.Interfaces;
using ChatAPI.Hubs;
using ChatAPI.Repos;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services;
using ChatAPI.Services.Interfaces;
using ChatAPI.Types;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChatAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbOptionsBuilder.UseSqlServer(connectionString);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<ApplicationDbContext>(cfg => cfg.UseSqlServer(
                connectionString),ServiceLifetime.Singleton, ServiceLifetime.Singleton);

            //JWT
            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
            builder.Services.AddSingleton(jwtOptions);
            builder.Services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken)
                                && path.StartsWithSegments("/Chat"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            //SignalR
            builder.Services.AddSignalR();

            //Helpers
            builder.Services.AddSingleton<JWTHelper>();


            //Repos
            builder.Services.AddSingleton<IUserRepo, UserRepo>();
            builder.Services.AddSingleton<IFriendshipRepo, FriendshipRepo>();
            builder.Services.AddSingleton<IGroupChatRepo, GroupChatRepo>();
            builder.Services.AddSingleton<IGroupMessageRepo, GroupMessageRepo>();
            builder.Services.AddSingleton<IGroupMessageStatusRepo, GroupMessageStatusRepo>();
            builder.Services.AddSingleton<IPrivateChatRepo, PrivateChatRepo>();
            builder.Services.AddSingleton<IPrivateMessageRepo, PrivateMessageRepo>();
            builder.Services.AddSingleton<IUserConnectionsRepo, UserConnectionsRepo>();
            builder.Services.AddSingleton<IUserPrivateChatRepo, UserPrivateChatRepo>();

            //Services
            builder.Services.AddSingleton<IPasswordManager, PasswordManager>();
            builder.Services.AddSingleton<IUserManager, UserManager>();
            builder.Services.AddSingleton<IPrivateChatService, PrivateChatService>();
            builder.Services.AddSingleton<IUserConnectionsManager, UserConnectionsManager>();
            builder.Services.AddSingleton<IChatHubManager, ChatHubManager>();
            builder.Services.AddSingleton<IPrivateMessageManager, PrivateMessageManager>();
            builder.Services.AddSingleton<IFriendshipManager, FriendshipManager>();


            //Auth
            builder.Services.AddSingleton<IAuthentication, Authentication>();

            //Event Handlers
            builder.Services.AddSingleton<NotificationsHandler>();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(oprtions =>
                {
                    oprtions.SwaggerEndpoint("/openapi/v1.json", "Demo API");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/Chat");

            app.UseStaticFiles();

            app.MapControllers();
            //Remove All Connection From Connection Table
            var dbContext = new ApplicationDbContext(dbOptionsBuilder.Options);
            var userConnectionsManager = new UserConnectionsManager(new UserConnectionsRepo(dbContext));
            userConnectionsManager.RemoveAllConnectionsAsync().Wait();
            dbContext.Dispose();

            app.Run();
        }
    }
}
