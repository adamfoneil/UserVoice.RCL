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

        protected override async Task AfterSaveAsync(IDbConnection connection, SaveAction action, Item model, IDbTransaction txn = null)
        {
            if (model.AssignToUserId.HasValue)
            {
                await ((UserVoiceDataContext)Context).AcceptanceRequests.MergeAsync(new AcceptanceRequest()
                {
                    ItemId = model.Id,
                    UserId = model.AssignToUserId.Value,
                });
            }
        }
    }
}
