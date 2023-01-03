using Dapper.QX.Extensions;
using UserVoice.Database;
using UserVoice.RCL.Service.Interfaces;
using UserVoice.RCL.Service.Queries;
using UserVoice.Service;

namespace UserVoice.RCL.Service.Abstract
{
    public abstract class IssueImporter : IIssueImporter
    {
        private readonly UserVoiceDataContext _dataContext;

        public IssueImporter(UserVoiceDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        protected abstract string SourceName { get; }

        /// <summary>
        /// be sure to set the Item.ExternalId and Item.ExternalUrl so it tracks correctly in the database
        /// </summary>        
        protected abstract Task<IEnumerable<Item>> GetActiveItemsAsync();

        protected abstract Task<IEnumerable<int>> GetInactiveItemIdsAsync();

        protected abstract TimeSpan TimeToLive { get; }

        public async Task MergeItemsAsync()
        {
            var lastMerge = await _dataContext.ExternalItemSources.GetWhereAsync(new { name = SourceName }) ??
                new ExternalItemSource() { Name = SourceName };

            if ((DateTime.UtcNow.Subtract(lastMerge.LastMerge)) < TimeToLive) return;

            var openItems = await GetActiveItemsAsync();

            foreach (var item in openItems.Where(row => row.ExternalId.HasValue && !string.IsNullOrEmpty(row.ExternalUrl)))
            {
                if (await _dataContext.ExternalItems.ExistsAsync(item.ExternalId.Value)) continue;
                await _dataContext.Items.SaveAsync(item);
            }

            var closedItems = await GetInactiveItemIdsAsync();

            foreach (var itemIdChunk in closedItems.Chunk(50))
            {
                await new CloseItems()
                {
                    ItemIds = itemIdChunk.ToDataTable()
                }.ExecuteAsync(_dataContext.GetConnection);
            }

            lastMerge.LastMerge = DateTime.UtcNow;
            await _dataContext.ExternalItemSources.SaveAsync(lastMerge);
        }
    }
}
