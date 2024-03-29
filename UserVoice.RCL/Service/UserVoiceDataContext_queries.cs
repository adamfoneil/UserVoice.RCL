﻿using UserVoice.RCL.Service.Queries;

namespace UserVoice.Service
{
    public partial class UserVoiceDataContext
    {
        /// <summary>
        /// use this to determine if there are any release notes ready to display for the given user,
        /// which can you can use to hide any componentry that presents the release note list
        /// </summary>
        public async Task<bool> HasReleaseNotesAsync(string userName)
        {
            var items = await new MyReleaseNotes() { UserName = userName }.ExecuteAsync(GetConnection);
            if (items.Any()) return true;
            
            var comments = await new MyReleaseNoteComments() { UserName = userName}.ExecuteAsync(GetConnection);
            if (comments.Any()) return true;

            return false;
        }
    }
}
