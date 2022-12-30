using Dapper.QX;

namespace UserVoice.RCL.Service.Queries
{
    internal class InsertUnreadComments : Query<int>
    {
        public InsertUnreadComments() : base(
            @"INSERT INTO [uservoice].[UnreadComment] (
                [CommentId], [UserId]
            ) SELECT
                @commentId, [u].[Id]
            FROM
                [uservoice].[User] [u]
            WHERE
                [u].[IsActive]=1 AND
                [u].[Id]<>@userId AND
                NOT EXISTS(SELECT 1 FROM [uservoice].[UnreadComment] WHERE [CommentId]=@commentId AND [UserId]=[u].[Id])")
        {
        }

        public int CommentId { get; set; }
        public int UserId { get; set; }
    }
}
