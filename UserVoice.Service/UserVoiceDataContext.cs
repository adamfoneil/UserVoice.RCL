using Dapper;
using Dapper.Repository.SqlServer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Reflection;
using UserVoice.Database;
using UserVoice.Service.Extensions;
using UserVoice.Service.Models;
using UserVoice.Service.Repositories;

namespace UserVoice.Service
{
    public class UserVoiceDataContext : SqlServerContext<User>
    {
        public UserVoiceDataContext(IOptions<ConnectionStrings> options, ILogger<UserVoiceDataContext> logger) : base(options.Value.UserVoice, logger)
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

        public Dictionary<ItemType, ItemTypeInfo> TypeInfo = new()
        {
            [ItemType.Issue] = new("stop", "color:maroon", "Issue", "Issues"),
            [ItemType.FeatureIdea] = new("lightbulb", "color:darkgreen", "Feature/Idea", "Features/Ideas"),
            [ItemType.TestCase] = new("science", "color:#cc33ff", "Test Case", "Test Cases")
        };

        public struct ItemTypeInfo
        {
            public ItemTypeInfo(string icon, string style, string singularText, string pluralText)
            {
                Icon = icon;
                Style = style;
                SingularText = singularText;
                PluralText = pluralText;
            }

            public string Icon { get; init; }
            public string Style { get; init; }
            public string SingularText { get; init; }
            public string PluralText { get; init; }
        }
    }
}