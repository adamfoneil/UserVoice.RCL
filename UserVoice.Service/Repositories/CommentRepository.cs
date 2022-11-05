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
            if (action == SaveAction.Insert && model.ItemStatus.HasValue)
            {
                var ctx = (UserVoiceDataContext)Context;
                var item = await ctx.Items.GetAsync(model.ItemId);
                item.StatusCommentId = model.Id;
                await ctx.Items.SaveAsync(item);
            }

            await base.AfterSaveAsync(connection, action, model, txn);
        }
    }
}
