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
            await base.BeforeSaveAsync(connection, action, model, txn);

            var context = Context;

            if (model.ExternalId.HasValue && !string.IsNullOrEmpty(model.ExternalUrl))
            {
                var externalItem = await context.ExternalItems.GetByExternalIdAsync(model.ExternalId.Value);
                if (externalItem != null) model.Id = externalItem.ItemId;
            }
        }

        protected override async Task AfterSaveAsync(IDbConnection connection, SaveAction action, Item model, IDbTransaction txn = null)
        {
            var context = Context;

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
                var externalItem = await context.ExternalItems.GetByExternalIdAsync(model.ExternalId.Value) ?? new ExternalItem()
                {
                    ExternalId = model.ExternalId.Value
                };

                externalItem.Url = model.ExternalUrl;
                externalItem.ItemId = model.Id;
                await context.ExternalItems.SaveAsync(externalItem);
            }
        }
    }
}
