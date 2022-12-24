using Dapper.QX;
using Dapper.QX.Attributes;
using UserVoice.Database;

namespace UserVoice.RCL.Service.Queries
{
    public class MyReleaseNotes : Query<Item>
    {
        public MyReleaseNotes() : base(
            @"IF NOT EXISTS(SELECT 1 FROM [uservoice].[ReleaseNoteMarker] WHERE [UserName]=@userName)
            BEGIN
                INSERT INTO [uservoice].[ReleaseNoteMarker] ([UserName], [VisibleAfter]) VALUES (@userName, '1/1/90')
            END;

            SELECT {top}
                [i].*
            FROM 
                [uservoice].[Item] [i]
                INNER JOIN [uservoice].[ReleaseNoteMarker] [m] ON [i].[DateCreated] > [m].[VisibleAfter] AND [m].[UserName]=@userName
            WHERE
                [i].[Type]=4 AND
                [i].[IsActive]=1
            ORDER BY 
                [i].[DateCreated] DESC")
        {
        }

        [Top]
        public int? Top { get; set; }

        public string UserName { get; set; } = default!;
    }
}
