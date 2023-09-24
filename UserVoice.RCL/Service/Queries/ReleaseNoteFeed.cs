using Dapper.QX;
using Dapper.QX.Attributes;

namespace UserVoice.RCL.Service.Queries;

public enum TemplateType
{
    ReleaseNote = 1,
    Comment = 2
}

public class ReleaseNoteFeedResult
{
    public TemplateType Template { get; set; }
    public int Id { get; set; }
    public int Type { get; set; }
    public string Title { get; set; } = default!;
    public string ItemBody { get; set; } = default!;
    public string CommentBody { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public DateTime DateCreated { get; set; }
}

public class ReleaseNoteFeed : Query<ReleaseNoteFeedResult>
{
    public ReleaseNoteFeed() : base(
        @"DECLARE @results TABLE (
            [Template] int NOT NULL,
            [Id] int NOT NULL,
            [Type] int NOT NULL,
            [Title] nvarchar(255) NOT NULL,
            [ItemBody] nvarchar(max) NOT NULL,
            [CommentBody] nvarchar(max) NULL,
            [DateCreated] datetime NOT NULL,
            [CreatedBy] nvarchar(50) NOT NULL
        )

        INSERT INTO @results (
            [Template], [Id], [Type], [Title], [ItemBody], [DateCreated], [CreatedBy]
        ) SELECT
            1, [Id], [Type], [Title], [Body], [DateCreated], [CreatedBy]
        FROM
            [uservoice].[Item]
        WHERE
            [Type]=4 AND
            [IsActive]=1
        ORDER BY
            [DateCreated] DESC

        INSERT INTO @results (
            [Template], [Id], [Type], [Title], [ItemBody], [CommentBody], [DateCreated], [CreatedBy]
        ) SELECT
            2, [i].[Id], [i].[Type], [i].[Title], [i].[Body], [c].[Body], [c].[DateCreated], [c].[CreatedBy]
        FROM
            [uservoice].[Comment] [c]
            INNER JOIN [uservoice].[Item] [i] ON [c].[ItemId] = [i].[Id]
        WHERE
            [c].[IsReleaseNote]=1
    
        SELECT * FROM @results ORDER BY [DateCreated] DESC {offset}")
    {            
    }

    [Offset(10)]
    public int? Page { get; set; }
}
