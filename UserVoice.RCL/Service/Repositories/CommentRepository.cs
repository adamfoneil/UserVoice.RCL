using AO.Models.Enums;
using System.Data;
using UserVoice.Database;

namespace UserVoice.Service.Repositories
{
    public class CommentRepository : BaseRepository<Comment>
    {
        public CommentRepository(UserVoiceDataContext context) : base(context)
        {
        }

        protected override async Task AfterSaveAsync(IDbConnection connection, SaveAction action, Comment model, IDbTransaction txn = null)
        {
            var ctx = (UserVoiceDataContext)Context;

            if (action == SaveAction.Insert && model.ItemStatus.HasValue)
            {                
                var item = await ctx.Items.GetAsync(model.ItemId);
                item.StatusCommentId = model.Id;
                await ctx.Items.SaveAsync(item);
            }

            if (model.AcceptanceRequestId.HasValue && model.IsRejected)
            {
                var ar = await ctx.AcceptanceRequests.GetAsync(model.AcceptanceRequestId.Value);
                ar.Response = Response.Rejected;
                await ctx.AcceptanceRequests.SaveAsync(ar);
            }

            await base.AfterSaveAsync(connection, action, model, txn);
        }
    }
}
