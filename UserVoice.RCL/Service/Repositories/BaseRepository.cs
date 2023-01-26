using AO.Models.Enums;
using AO.Models.Interfaces;
using Dapper.Repository;
using System.Data;
using UserVoice.Database;
using UserVoice.Database.Conventions;

namespace UserVoice.Service.Repositories
{
    public class BaseRepository<TModel> : Repository<UserVoiceDataContext, User, TModel, int> where TModel : IModel<int>
    {
        public BaseRepository(UserVoiceDataContext context) : base(context)
        {
        }

        protected override async Task BeforeSaveAsync(IDbConnection connection, SaveAction action, TModel model, IDbTransaction txn = null)
        {
            if (model is BaseEntity baseEntity)
            {
                switch (action)
                {
                    case SaveAction.Insert:
                        baseEntity.CreatedBy = Context.User.Name;
                        baseEntity.DateCreated = Context.User.LocalTime;
                        break;

                    case SaveAction.Update:
                        baseEntity.ModifiedBy = Context.User.Name;
                        baseEntity.DateModified = Context.User.LocalTime;
                        break;
                }
            }


            await base.BeforeSaveAsync(connection, action, model, txn);
        }
    }
}
