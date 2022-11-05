using AO.Models;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public enum CommentType
    {
        /// <summary>
        /// requested feature was implemented
        /// </summary>
        Done,
        /// <summary>
        /// requested feature is on the roadmap, has issue created
        /// </summary>
        Planned,
        /// <summary>
        /// won't implement for some stated reason
        /// </summary>
        NotPlanned,
        /// <summary>
        /// need more info before proceeding
        /// </summary>
        NeedsInfo
    }

    public class Comment : BaseEntity
    {
        [References(typeof(Item))]
        public int ItemId { get; set; }

        public CommentType? Type { get; set; }

        public string Body { get; set; }
    }
}
