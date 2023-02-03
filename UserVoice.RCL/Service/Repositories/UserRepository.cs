using UserVoice.Database;
using UserVoice.Service;
using UserVoice.Service.Repositories;

namespace UserVoice.RCL.Service.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(UserVoiceDataContext dataContext) : base(dataContext)
        {
        }

        public async Task MergeUsersAsync(IEnumerable<User> users)
        {
            foreach (var user in users) await MergeAsync(user);
        }

        public async Task<User> GetUserByEmailAsync(string email) => await GetWhereAsync(new { email }) ?? new User()
        {
            Name = email,
            Email = email
        };
    }
}
