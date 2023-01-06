namespace UserVoice.RCL.Service.Models
{
    public class GitHubIssueImportOptions
    {
        public int TimeToLiveMinutes { get; set; }
        public string RepositoryName { get; set; } = default!;
    }
}
