using CopyObjectLibrary;
using Dapper.QX;
using Dapper.QX.Attributes;
using Microsoft.AspNetCore.Components;
using UserVoice.Database;

namespace UserVoice.Service.Queries
{
    public enum ListItemsSortOptions
    {
        LatestModifedOrAdded,
        LatestStatusChange,
        MostVotes,
        MostUpvoted,
        LatestComment,
        UnreadComments,
        Priority
    }

    public class ListItemsResult
    {
        public int Id { get; set; }
        public ItemType Type { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public int? StatusCommentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public string? ModifiedBy { get; set; }
        public ItemStatus? ItemStatus { get; set; }
        public string? StatusBody { get; set; }
        public int TotalUpvotes { get; set; }
        public int TotalDownvotes { get; set; }
        public int TotalVotes { get; set; }
        public DateTime? StatusDate { get; set; }
        public int AcceptanceRequestCount { get; set; }
        public DateTime? LatestCommentDate { get; set; }
        public string? LatestCommentUser { get; set; }
        public int UnreadCommentCount { get; set; }
        public int? ExternalId { get; set; }
        public string? ExternalUrl { get; set; }
        public int? Priority { get; set; }
        public bool AllowPriority => Item.AllowPriority(Type);

        public DateTime PostDate => DateModified ?? DateCreated;
        public int DisplayId => ExternalId.HasValue ? ExternalId.Value : Id;

        public string DateInfo()
        {
            var result = $"Created {DateCreated:ddd M/d/yy h:mm t}";
            if (DateModified.HasValue) result += $", modified {DateModified:ddd M/d/yy h:mm t}";
            return result;
        }

        public string UnreadCssClass => (UnreadCommentCount > 0) ? "font-weight-bold" : string.Empty;
        public MarkupString UnreadMarkup(string? input) => (UnreadCommentCount > 0) ? new MarkupString($"<strong>{input}</strong>") : new MarkupString(input);
        public bool NeedsSignOff => Type == ItemType.TestCase && (AcceptanceRequestCount == 0);

        public static implicit operator Item(ListItemsResult row) => row.CopyAs<Item>();
    }

    public class ListItems : Query<ListItemsResult>
    {
        public ListItems() : base(
            @"DECLARE @latestCommentIds TABLE (
                [ItemId] int NOT NULL PRIMARY KEY,
                [CommentId] int NOT NULL
            );

            INSERT INTO @latestCommentIds (
                [ItemId], [CommentId]
            ) SELECT
                [ItemId], MAX([Id]) AS [LatestCommentId]
            FROM 
                [uservoice].[Comment]
            GROUP BY 
                [ItemId];

            WITH [source] AS (
                SELECT 
                    [i].*, 
                    [ip].[Order] AS [Priority],
                    [ei].[ExternalId], [ei].[Url] AS [ExternalUrl],
                    [c].[ItemStatus], [c].[Body] AS [StatusBody], COALESCE([c].[DateModified], [c].[DateCreated]) AS [StatusDate],
                    (
                        SELECT COUNT(1) 
                        FROM 
                            [uservoice].[UnreadComment] [uc] 
                            INNER JOIN [uservoice].[Comment] [c] ON [uc].[CommentId]=[c].[Id] AND [uc].[UserId]=@userId
                        WHERE 
                            [c].[ItemId]=[i].[Id]
                    ) AS [UnreadCommentCount]
                FROM 
                    [uservoice].[Item] [i]
                    LEFT JOIN [uservoice].[Comment] [c] ON [i].[StatusCommentId]=[c].[Id]
                    LEFT JOIN [uservoice].[ExternalItem] [ei] ON [i].[Id]=[ei].[ItemId]
                    LEFT JOIN [uservoice].[ItemPriority] [ip] ON [i].[Id]=[ip].[ItemId]
                {where}
            ), [votes] AS (
                SELECT 
                    [src].*, 
                    COALESCE([votes].[TotalUpvotes], 0) AS [TotalUpvotes],
                    COALESCE([votes].[TotalDownvotes], 0) AS [TotalDownvotes],
                    [lc].[LatestCommentDate], [lc].[LatestCommentUser]
                FROM 
                    [source] [src]
                    LEFT JOIN (
                        SELECT 
                            [v].[ItemId], 
                            COUNT(CASE [v].[Upvoted] WHEN 1 THEN 1 ELSE NULL END) AS [TotalUpvotes],
                            COUNT(CASE [v].[Upvoted] WHEN 0 THEN 1 ELSE NULL END) AS [TotalDownvotes]
                        FROM [uservoice].[Vote] [v]
                        GROUP BY [v].[ItemId]
                    ) [votes] ON [src].[Id]=[votes].[ItemId]
                    LEFT JOIN (
                        SELECT [c].[ItemId], [c].[DateCreated] AS [LatestCommentDate], [c].[CreatedBy] AS [LatestCommentUser]
                        FROM @latestCommentIds [lc]
                        INNER JOIN [uservoice].[Comment] [c] ON [lc].[CommentId]=[c].[Id]
                    ) [lc] ON [src].[Id]=[lc].[ItemId]
            ) SELECT 
                [i].*, 
                COALESCE([i].[TotalUpvotes], 0) + COALESCE([i].[TotalDownvotes], 0) AS [TotalVotes],
                (SELECT COUNT(1) FROM [uservoice].[AcceptanceRequest] WHERE [ItemId]=[i].[Id]) AS [AcceptanceRequestCount]                
            FROM 
                [votes] [i]
            ORDER BY 
                {orderBy} {offset}")
        {
        }

        [Offset(30)]
        public int? Page { get; set; } = 0;

        public int UserId { get; set; }

        [Where("[i].[IsActive]=@isActive")]
        public bool? IsActive { get; set; } = true;

        [Where("[i].[Type]=@type")]
        public ItemType? Type { get; set; }

        [Case(true, "[i].[Type] IN (1, 2)")]
        [Case(false, "[i].[Type] IN (3, 4)")]
        public bool? AllowsPriority { get; set; }

        [Case(Database.Response.Unassigned, "NOT EXISTS(SELECT 1 FROM [uservoice].[AcceptanceRequest] WHERE [ItemId]=[i].[Id])")]
        [Where("EXISTS(SELECT 1 FROM [uservoice].[AcceptanceRequest] WHERE [Response]=@response AND [ItemId]=[i].[Id])")]
        public Response? Response { get; set; }

        [Where("EXISTS(SELECT 1 FROM [uservoice].[AcceptanceRequest] WHERE [UserId]=@assignedToUserId AND [ItemId]=[i].[Id])")]
        public int? AssignedToUserId { get; set; }

        [Where("[i].[Title] LIKE CONCAT('%', @search, '%')")]
        public string? Search { get; set; }

        [Where("([i].[Id]=@itemId OR [ei].[ExternalId]=@itemId)")]
        public int? ItemId { get; set; }

        [OrderBy(ListItemsSortOptions.LatestModifedOrAdded, "COALESCE([i].[DateModified], [i].[DateCreated]) DESC")]
        [OrderBy(ListItemsSortOptions.LatestStatusChange, "[StatusDate] DESC")]
        [OrderBy(ListItemsSortOptions.MostVotes, "[TotalVotes] DESC")]
        [OrderBy(ListItemsSortOptions.MostUpvoted, "[TotalUpvotes] DESC")]
        [OrderBy(ListItemsSortOptions.LatestComment, "[LatestCommentDate] DESC")]
        [OrderBy(ListItemsSortOptions.UnreadComments, "[UnreadCommentCount] DESC")]
        [OrderBy(ListItemsSortOptions.Priority, "COALESCE([Priority], 1000) ASC")]
        public ListItemsSortOptions Sort { get; set; } = ListItemsSortOptions.LatestModifedOrAdded;
    }
}
