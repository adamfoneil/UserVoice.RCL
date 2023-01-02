using Dapper.QX;
using Dapper.QX.Attributes;
using System.Data;

namespace UserVoice.RCL.Service.Queries
{
    public class CloseItems : Query<int>
    {
        public CloseItems() : base(
            @"UPDATE [i] SET
                [IsActive]=0
            FROM
                [uservoice].[Item] [i]
                INNER JOIN @itemIds [ids] ON [i].[Id]=[ids].[Id]
            WHERE
                [i].[IsActive]=1")
        {
        }

        [TableType("uservoice.IdList")]
        public DataTable? ItemIds { get; set; }
    }
}
