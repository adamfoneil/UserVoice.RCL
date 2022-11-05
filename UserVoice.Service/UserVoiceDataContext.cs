using Dapper;
using Dapper.Repository.SqlServer;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;
using UserVoice.Database;
using UserVoice.Service.Extensions;
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

        public async Task CreateSchemaIfNotExistsAsync()
        {
            using var cn = GetConnection();
            if (!await cn.SchemaExistsAsync("uservoice"))
            {
                var script = GetResource("Resources.DbSchema.sql");
                var commands = script.Split("GO\rn");
                foreach (var cmd in commands) await cn.ExecuteAsync(cmd);
            }
        }

        private string GetResource(string name)
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream(name) ?? throw new ApplicationException($"Stream name {name} was not found");
            return new StreamReader(stream).ReadToEnd();
        }
    }
}