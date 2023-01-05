using UserVoice.Database;
using UserVoice.Service;
using UserVoice.Service.Repositories;

namespace UserVoice.RCL.Service.Repositories
{
    public class ExternalItemRepository : BaseRepository<ExternalItem>
    {
        public ExternalItemRepository(UserVoiceDataContext dataContext) : base(dataContext)
        {
        }

        public async Task<bool> ExistsAsync(int externalId)
        {
            var externalItem = await GetByExternalIdAsync(externalId);
            return (externalItem is not null);
        }

        public async Task<ExternalItem> GetByExternalIdAsync(int externalId) => await GetWhereAsync(new { externalId });

        public async Task LinkExistingAsync(Item item)
        {
            if (item.ExternalId.HasValue)
            {
                var existing = await GetByExternalIdAsync(item.ExternalId.Value);
                if (existing is not null && item.Id == 0)
                {
                    item.Id = existing.ItemId;
                }
            }            
        }
    }
}
