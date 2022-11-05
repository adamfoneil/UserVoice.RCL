using System.ComponentModel.DataAnnotations;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public enum Role
    {
        /// <summary>
        /// can make requests, vote on features, post comments (bot not select type)
        /// </summary>
        User,
        /// <summary>
        /// developer or product rep who can post comments and select the type
        /// </summary>
        ProductOwner,
        /// <summary>
        /// someone who by default signs off on UAT items
        /// </summary>
        SignOffUser
    }

    /// <summary>
    /// users will be imported from the host application and periodically synced
    /// </summary>
    public class User : BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; } = default!;

        [MaxLength(50)]
        public string Email { get; set; } = default!;

        public Role Role { get; set; }
    }
}
