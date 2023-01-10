using Dapper.QX;
using Dapper.QX.Attributes;
using System.Data;

namespace UserVoice.RCL.Service.Queries
{
    public class CloseExternalItems : Query<int>
    {
        public CloseExternalItems() : base(
            @"UPDATE [i] SET
                [IsActive]=0
            FROM
                [uservoice].[Item] [i]
                INNER JOIN [uservoice].[ExternalItem] [ei] ON [i].[Id]=[ei].[ItemId]                
                INNER JOIN @itemIds [ids] ON [ei].[ExternalId]=[ids].[Id]                
            WHERE
                [i].[IsActive]=1")
        {
        }

        [TableType("uservoice.IdList")]
        public DataTable? ItemIds { get; set; }
    }
}
