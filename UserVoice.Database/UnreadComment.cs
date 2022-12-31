using AO.Models;
using AO.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace UserVoice.Database
{
    /// <summary>
    /// inserted automatically whenever a comment is created to create an unread flag.
    /// User may mark comments as read to delete these rows
    /// </summary>
    [Schema("uservoice")]
    public class UnreadComment : IModel<int>
    {
        public int Id { get; set; }

        [Key]
        [References(typeof(Comment), CascadeDelete = true)]
        public int CommentId { get; set; }

        [Key]
        [References(typeof(User), CascadeDelete = true)]
        public int UserId { get; set; }
    }
}
