using UserVoice.Database;
using UserVoice.Service;
using UserVoice.Service.Repositories;

namespace UserVoice.RCL.Service.Repositories
{
    public class ReleaseNoteMarkerRepository : BaseRepository<ReleaseNoteMarker>
    {
        public ReleaseNoteMarkerRepository(UserVoiceDataContext dataContext) : base(dataContext)
        {
        }

        public async Task MarkNowAsync(User user) => await ((UserVoiceDataContext)Context).ReleaseNoteMarkers.MergeAsync(new ReleaseNoteMarker()
        {
            UserName = user.Name,
            VisibleAfter = user.LocalTime
        });
    }
}
