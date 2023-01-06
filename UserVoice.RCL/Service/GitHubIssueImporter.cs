using GitHubApiClient;
using GitHubApiClient.Models.Requests;
using GitHubApiClient.Models.Responses;
using Markdig;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using UserVoice.Database;
using UserVoice.RCL.Service.Abstract;
using UserVoice.RCL.Service.Models;
using UserVoice.Service;

namespace UserVoice.RCL.Service
{
    public class GitHubIssueImporter : IssueImporter
    {        
        private readonly GitHubClient _client;
        private readonly GitHubIssueImportOptions _options;

        private IEnumerable<Issue> _allIssues = Enumerable.Empty<Issue>();

        public GitHubIssueImporter(IOptions<GitHubIssueImportOptions> options, GitHubClient client, UserVoiceDataContext dataContext) : base(dataContext)
        {
            _options = options.Value;
            _client = client;
        }

        protected override string SourceName => "GitHub";

        protected override TimeSpan TimeToLive => TimeSpan.FromMinutes(_options.TimeToLiveMinutes);

        protected override async Task<IEnumerable<int>> GetInactiveItemIdsAsync()
        {
            var closed = await _client.GetAllIssuesAsync(_options.RepositoryName, new IssuesQuery()
            {
                State = IssueState.Closed
            });

            // if the issue is open, but has no one assigned, I consider this an inactive item
            var allResults = closed.Concat(_allIssues.Where(issue => !issue.assignees.Any()));

            return allResults.Select(issue => issue.number);
        }

        protected override async Task<IEnumerable<Item>> GetActiveItemsAsync()
        {
            _allIssues = await _client.GetAllIssuesAsync(_options.RepositoryName, new IssuesQuery()
            {                
                State = IssueState.All
            });

            var titlesByNumber = _allIssues.ToDictionary(issue => issue.number, issue => issue.title);

            return _allIssues.Where(issue => issue.assignees.Any() && issue.state.Equals("open")).Select(issue =>
            {
                var body = issue.body + $"\r\n\r\nAssigned to: {string.Join(", ", issue.assignees.Select(u => u.login))}";

                ReplaceIssueRefs(ref body, titlesByNumber);

                return new Item()
                {
                    Type = ItemType.Issue,
                    ExternalId = issue.number,
                    ExternalUrl = issue.html_url,
                    Title = issue.title,
                    Body = Markdown.ToHtml(body)
                };
            });
        }

        private void ReplaceIssueRefs(ref string body, Dictionary<int, string> titlesByNumber)
        {
            var result = Regex.Replace(body, @"\s#(\d*)\s", InsertTitle);

            string InsertTitle(Match match) =>
                (int.TryParse(match.Groups[1].Value, out int issueNum) && titlesByNumber.ContainsKey(issueNum)) ? 
                    $" {titlesByNumber[issueNum]} [# {issueNum}] " : 
                    string.Empty;

            body = result;
        }        
    }
}
