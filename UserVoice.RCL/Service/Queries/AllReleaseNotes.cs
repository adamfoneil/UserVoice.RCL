using Dapper.QX;
using Dapper.QX.Attributes;
using UserVoice.Database;

namespace UserVoice.RCL.Service.Queries
{
    public class AllReleaseNotes : Query<Item>
    {
        public AllReleaseNotes() : base(
            @"SELECT 
                [i].*
            FROM 
                [uservoice].[Item] [i]
            WHERE
                [Type]=4 AND
                [IsActive]=1
            ORDER BY
                [DateCreated] DESC {offset}")
        {
        }

        [Offset(10)]
        public int? Page { get; set; }
    }
}
