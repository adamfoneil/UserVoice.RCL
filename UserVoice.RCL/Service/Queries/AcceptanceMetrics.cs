using Dapper.QX;
using UserVoice.Database;

namespace UserVoice.Service.Queries
{
    public class AcceptanceMetricsResult
    {
        public Response Response { get; set; }
        public int Count { get; set; }
        public decimal Percent { get; set; }
    }

    public class AcceptanceMetrics : Query<AcceptanceMetricsResult>
    {
        public AcceptanceMetrics() : base(
            @"DECLARE @results TABLE (
                [Response] int NOT NULL,
                [Count] int NOT NULL,
                [Percent] decimal(4,3) NOT NULL
            )

            DECLARE @total decimal
            SELECT @total = COUNT(1) 
            FROM [uservoice].[AcceptanceRequest] [ar]
            INNER JOIN [uservoice].[Item] [i] ON [ar].[ItemId]=[i].[Id]
            WHERE [i].[IsActive]=@isActive

            DECLARE @unassigned int
            SELECT @unassigned = COUNT(1)
            FROM [uservoice].[Item] [i]
            WHERE [IsActive]=@isActive AND [Type]=3 AND NOT EXISTS(SELECT 1 FROM [uservoice].[AcceptanceRequest] WHERE [ItemId]=[i].[Id])

            SET @total = @total + @unassigned

            INSERT INTO @results (
                [Response], [Count], [Percent]
            ) SELECT
                [Response], 
                COUNT(1) AS [Count],
                COUNT(1) / @total
            FROM
                [uservoice].[AcceptanceRequest] [ar]
                INNER JOIN [uservoice].[Item] [i] ON [ar].[ItemId]=[i].[Id]
            WHERE
                [i].[IsActive]=@isActive
            GROUP BY
                [Response]
    
            INSERT INTO @results (
                [Response], [Count], [Percent]
            ) VALUES (
                -1, @unassigned, @unassigned / @total
            )
    
            SELECT * FROM @results")
        {
        }

        public bool IsActive { get; set; }
    }
}
