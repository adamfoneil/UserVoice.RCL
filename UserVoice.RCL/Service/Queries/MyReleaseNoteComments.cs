using Dapper.QX;
using Dapper.QX.Attributes;
using UserVoice.Database;

namespace UserVoice.RCL.Service.Queries;

public class MyReleaseNoteCommentsResult
{
    public ItemType Type { get; set; }
	public string Title { get; set; } = default!;
	public string ItemBody { get; set; } = default!;
	public bool IsActive { get; set; }
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int? ItemStatus { get; set; }
	public string Body { get; set; } = default!;
    public DateTime DateCreated { get; set; }
	public string CreatedBy { get; set; } = default!;
    public DateTime? DateModified { get; set; }
	public string ModifiedBy { get; set; } = default!;
    public bool IsReleaseNote { get; set; }
}

public class MyReleaseNoteComments : Query<MyReleaseNoteCommentsResult>
{
	public MyReleaseNoteComments() : base(
		@"IF NOT EXISTS(SELECT 1 FROM [uservoice].[ReleaseNoteMarker] WHERE [UserName]=@userName)
		BEGIN
			INSERT INTO [uservoice].[ReleaseNoteMarker] ([UserName], [VisibleAfter]) VALUES (@userName, '1/1/90')
		END;

		SELECT {top}
			[i].[Type],
			[i].[Title],
			[i].[Body] AS [ItemBody],
			[i].[IsActive],
			[c].*
		FROM 
			[uservoice].[Comment] [c]
			INNER JOIN [uservoice].[ReleaseNoteMarker] [m] ON [c].[DateCreated] > [m].[VisibleAfter] AND [m].[UserName]=@userName
			INNER JOIN [uservoice].[Item] [i] ON [c].[ItemId] = [i].[Id]
		WHERE
			[c].[IsReleaseNote]=1
		ORDER BY 
			[c].[DateCreated] DESC")
	{            
	}

	[Top]
	public int? Top { get; set; }

	public string UserName { get; set; } = default!;
}
