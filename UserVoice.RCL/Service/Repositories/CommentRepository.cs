using AO.Models.Enums;
using System.Data;
using UserVoice.Database;
using UserVoice.RCL.Service.Queries;

namespace UserVoice.Service.Repositories
{
    public class CommentRepository : BaseRepository<Comment>
    {
        public CommentRepository(UserVoiceDataContext context) : base(context)
        {
        }

        protected override async Task AfterSaveAsync(IDbConnection connection, SaveAction action, Comment model, IDbTransaction txn = null)
        {
            var ctx = Context;

            if (action == SaveAction.Insert)
            {
                await new InsertUnreadComments()
                {
                    CommentId = model.Id,
                    ExcludeUserId = Context.User.Id
                }.ExecuteAsync(connection);

                if (model.ItemStatus.HasValue)
                {
                    var item = await ctx.Items.GetAsync(model.ItemId);
                    item.StatusCommentId = model.Id;
                    await ctx.Items.SaveAsync(item);
                }
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
