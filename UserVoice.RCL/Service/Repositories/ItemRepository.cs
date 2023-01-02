using AO.Models.Enums;
using System.Data;
using UserVoice.Database;
using UserVoice.Service;
using UserVoice.Service.Repositories;

namespace UserVoice.RCL.Service.Repositories
{
    public class ItemRepository : BaseRepository<Item>
    {
        public ItemRepository(UserVoiceDataContext dataContext) : base(dataContext)
        {
        }

        protected override async Task BeforeSaveAsync(IDbConnection connection, SaveAction action, Item model, IDbTransaction txn = null)
        {
            var context = (UserVoiceDataContext)Context;

            if (model.ExternalId.HasValue && !string.IsNullOrEmpty(model.ExternalUrl))
            {
                var externalItem = await context.ExternalItems.GetWhereAsync(new { externalId = model.ExternalId.Value });
                if (externalItem != null) model.Id = externalItem.ItemId;
            }
        }

        protected override async Task AfterSaveAsync(IDbConnection connection, SaveAction action, Item model, IDbTransaction txn = null)
        {
            var context = (UserVoiceDataContext)Context;

            if (model.AssignToUserId.HasValue)
            {
                await context.AcceptanceRequests.MergeAsync(new AcceptanceRequest()
                {
                    ItemId = model.Id,
                    UserId = model.AssignToUserId.Value,
                });
            }

            if (model.ExternalId.HasValue && !string.IsNullOrEmpty(model.ExternalUrl))
            {
                await context.ExternalItems.MergeAsync(new ExternalItem()
                {
                    ItemId = model.Id,
                    ExternalId = model.ExternalId.Value,
                    Url = model.ExternalUrl
                });
            }
        }
    }
}
