using ChatAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;

namespace ChatAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Friend> Friends { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<GroupMessageStatus> GroupMessageStatuses { get; set; }
        public DbSet<PrivateChat> PrivateChats { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<UserGroupChat> UserGroupChats { get; set; }
        public DbSet<UserPrivateChat> UserPrivateChats { get; set; }


        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Friend>().HasKey(x => new { x.RequestorUserId, x.RespondentUserId });
            modelBuilder.Entity<GroupMessageStatus>().HasKey(x => new { x.GroupChatId, x.UserId });
            modelBuilder.Entity<UserGroupChat>().HasKey(x => new { x.GroupChatId, x.UserId });
            modelBuilder.Entity<UserPrivateChat>().HasKey(x => new { x.PrivateChatId, x.UserId });
            modelBuilder.Entity<UserConnection>().HasKey(x => new { x.UserId, x.SignalRConnectionId });
        }
    }
}
