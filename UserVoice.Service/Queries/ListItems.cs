using Dapper.QX;
using Dapper.QX.Attributes;
using UserVoice.Database;

namespace UserVoice.Service.Queries
{
    public enum ListItemsSortOptions
    {
        LatestModifedOrAdded,
        LatestStatusChange,
        MostVotes,
        MostUpvoted
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
    }

    public class ListItems : Query<ListItemsResult>
    {
        public ListItems() : base(
            @"WITH [source] AS (
                SELECT [i].*, [c].[ItemStatus], [c].[Body] AS [StatusBody], COALESCE([c].[DateModified], [c].[DateCreated]) AS [StatusDate]
                FROM 
                    [uservoice].[Item] [i]
                    LEFT JOIN [uservoice].[Comment] [c] ON [i].[StatusCommentId]=[c].[Id]
                {where}
            ), [votes] AS (
                SELECT 
                    [src].*, 
                    COALESCE([votes].[TotalUpvotes], 0) AS [TotalUpvotes],
                    COALESCE([votes].[TotalDownvotes], 0) AS [TotalDownvotes]
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
            ) SELECT 
                [i].*, 
                COALESCE([i].[TotalUpvotes], 0) + COALESCE([i].[TotalDownvotes], 0) AS [TotalVotes]
            FROM 
                [votes] [i]
            ORDER BY 
                {sort}")
        {
        }

        [Where("[i].[IsActive]=@isActive")]
        public bool? IsActive { get; set; } = true;

        [OrderBy(ListItemsSortOptions.LatestModifedOrAdded, "COALESCE([i].[DateModified], [i].[DateCreated]) DESC")]
        [OrderBy(ListItemsSortOptions.LatestStatusChange, "[StatusDate] DESC")]
        [OrderBy(ListItemsSortOptions.MostVotes, "[TotalVotes] DESC")]
        [OrderBy(ListItemsSortOptions.MostUpvoted, "[TotalUpvotes] DESC")]
        public ListItemsSortOptions Sort { get; set; }
    }
}
