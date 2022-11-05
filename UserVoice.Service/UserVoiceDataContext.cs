using Dapper.Repository.SqlServer;
using Microsoft.Extensions.Logging;
using System.Data;
using UserVoice.Database;
using UserVoice.Service.Repositories;

namespace UserVoice.Service
{
    public class UserVoiceDataContext : SqlServerContext<User>
    {
        public UserVoiceDataContext(string connectionString, ILogger<UserVoiceDataContext> logger) : base(connectionString, logger)
        {
        }

        /// <summary>
        /// host application will set this directly
        /// </summary>
        public string CurrentUser { get; set; } = default!;

        protected override Task<User> QueryUserAsync(IDbConnection connection)
        {
            return base.QueryUserAsync(connection);
        }

        public BaseRepository<AcceptanceRequest> AcceptanceRequests => new BaseRepository<AcceptanceRequest>(this);
        public BaseRepository<Comment> Comments => new BaseRepository<Comment>(this);
        public BaseRepository<Item> Items => new BaseRepository<Item>(this);
        public BaseRepository<Vote> Votes => new BaseRepository<Vote>(this);
        public BaseRepository<User> Users => new BaseRepository<User>(this);

        public async Task MergeUsersAsync(IEnumerable<User> users)
        {
            foreach (var user in users) await Users.MergeAsync(user);            
        }
    }
}