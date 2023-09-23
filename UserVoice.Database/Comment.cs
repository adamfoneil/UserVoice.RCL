using AO.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public enum ItemStatus
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

        /// <summary>
        /// only ProductOwner users may select an item status
        /// </summary>
        public ItemStatus? ItemStatus { get; set; }

        [Required]
        public string Body { get; set; }

        /// <summary>
        /// if true, then this shows on the ReleaseNotesCompactList
        /// </summary>
        public bool IsReleaseNote { get; set; }

        public static bool AllowStatus(Role role) => role == Role.ProductOwner;

        [NotMapped]
        public int? AcceptanceRequestId { get; set; }

        [NotMapped]
        public bool IsRejected { get; set; }

        [NotMapped]
        public int? UnreadCommentId { get; set; }
    }
}
