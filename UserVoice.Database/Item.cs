using AO.Models;
using AO.Models.Enums;
using System.ComponentModel.DataAnnotations;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public class Item : BaseEntity
    {
        [Required]
        [MaxLength(255)]        
        public string Title { get; set; }

        public string Body { get; set; }

        /// <summary>
        /// most recent comment type
        /// </summary>
        [SaveAction(SaveAction.None)]
        public CommentType? CommentType { get; set; }

        /// <summary>
        /// true means the item is visible to everyone.
        /// false would mean several things:
        /// - an acceptance request not ready for voting
        /// - feedback acted upon, a feature implemented, and therefore not open for voting
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}