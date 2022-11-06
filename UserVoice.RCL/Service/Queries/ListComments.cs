using Dapper.QX;
using Dapper.QX.Attributes;
using System.Data;
using UserVoice.Database;

namespace UserVoice.Service.Queries
{
    public class ListComments : Query<Comment>
    {
        public ListComments() : base(
            @"SELECT 
                [c].*
            FROM 
                [uservoice].[Comment] [c]
                INNER JOIN @itemIds [i] ON [c].[ItemId]=[i].[Id]
            ORDER BY
                [c].[DateCreated] ASC")
        {
        }

        [TableType("uservoice.IdList")]
        public DataTable? ItemIds { get; set; }
    }
}
