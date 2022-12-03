using Dapper.QX;
using Dapper.QX.Attributes;
using System.Data;
using UserVoice.Database;

namespace UserVoice.Service.Queries
{
    public class ListAcceptanceRequests : Query<AcceptanceRequest>
    {
        public ListAcceptanceRequests() : base(
            @"SELECT 
                [ar].*, [u].[Name] AS [UserName], [u].[Email]
            FROM
                [uservoice].[AcceptanceRequest] [ar]
                INNER JOIN [uservoice].[User] [u] ON [ar].[UserId]=[u].[Id]
                INNER JOIN @itemIds [i] ON [ar].[ItemId]=[i].[Id]")
        {
        }

        [TableType("uservoice.IdList")]
        public DataTable? ItemIds { get; set; }
    }
}
