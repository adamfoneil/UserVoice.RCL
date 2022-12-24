using Dapper;
using Dapper.Repository.SqlServer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Reflection;
using UserVoice.Database;
using UserVoice.RCL.Service.Repositories;
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
        public CommentRepository Comments => new CommentRepository(this);
        public ItemRepository Items => new ItemRepository(this);
        public BaseRepository<Vote> Votes => new BaseRepository<Vote>(this);
        public UserRepository Users => new UserRepository(this);

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
            [ItemType.Issue] = new("feedback", "color:maroon", "Issue", "Issues"),
            [ItemType.FeatureIdea] = new("lightbulb", "color:darkgreen", "Feature/Idea", "Features/Ideas"),
            [ItemType.TestCase] = new("science", "color:#cc33ff", "Test Case", "Test Cases"),
            [ItemType.ReleaseNote] = new("campaign", "color:#FF9025", "Release Note", "Release Notes")
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
            public string PluralTextWithCount(int count) => $"{PluralText} ({count})";
        }

        public Dictionary<ItemStatus, ItemStatusInfo> StatusInfo = new()
        {
            [ItemStatus.Done] = new("check_circle", "color:darkgreen", "Done"),
            [ItemStatus.Planned] = new("pending", "color:lightblue", "Will Do"),
            [ItemStatus.NotPlanned] = new("do_not_disturb", "color:gray", "Not Planned"),
            [ItemStatus.NeedsInfo] = new("help", "color:#C97CFF", "Has Questions")            
        };

        public struct ItemStatusInfo
        {
            public ItemStatusInfo(string icon, string style, string text)
            {
                Icon = icon;
                Style = style;
                Text = text;
            }

            public string Icon { get; init; }
            public string Style { get; init; }
            public string Text { get; init; }
        }

        public Dictionary<Response, AcceptanceRequestResponseInfo> ResponseInfo = new()
        {
            [Response.Pending] = new("pending", "color:lightgray", "Pending"),
            [Response.Accepted] = new("assignment_turned_in", "color:green", "Accepted"),
            [Response.Rejected] = new("do_not_disturb_on", "color:red", "Rejected")
        };

        public struct AcceptanceRequestResponseInfo
        {
            public AcceptanceRequestResponseInfo(string icon, string style, string text)
            {
                Icon = icon;
                Style = style;
                Text = text;
            }

            public string Icon { get; init; }
            public string Style { get; init; }
            public string Text { get; init; }
        }
    }
}