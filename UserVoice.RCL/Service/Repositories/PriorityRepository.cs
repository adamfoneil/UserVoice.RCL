using Dapper;
using UserVoice.Database;
using UserVoice.Service;
using UserVoice.Service.Repositories;

namespace UserVoice.RCL.Service.Repositories;

public class PriorityRepository : BaseRepository<ItemPriority>
{
    public PriorityRepository(UserVoiceDataContext dataContext) : base(dataContext)
    {        
    }

    public async Task SetAsync(int itemId, int? value)
    {
        if (!value.HasValue)
        {
            using var cn = Context.GetConnection();
            await cn.ExecuteAsync("DELETE [uservoice].[ItemPriority] WHERE [ItemId]=@itemId", new { itemId });
            return;
        }

        await MergeAsync(new() { ItemId = itemId, Order = value.Value });
    }
}
