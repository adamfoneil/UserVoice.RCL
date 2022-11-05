using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public class FeatureVote : BaseEntity
    {
        [Key]
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        [Key]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// converted to int (+1 or -1) with
        /// ABS([Upvoted]*-1)
        /// </summary>
        public bool Upvoted { get; set; }
    }
}
