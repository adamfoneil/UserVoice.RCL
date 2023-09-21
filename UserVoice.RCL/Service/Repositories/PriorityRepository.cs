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
        ItemPriority row = new() { ItemId = itemId, Order = value };

        if (value == 0)
        {
            await DeleteAsync(row);
            return;
        }

        await MergeAsync(row);
    }
}
