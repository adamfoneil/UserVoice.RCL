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

    }
}
