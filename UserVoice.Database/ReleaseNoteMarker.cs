using AO.Models;
using AO.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace UserVoice.Database
{
    /// <summary>
    /// marks the timestamp before which release notes are hidden from a user's changelog view
    /// </summary>
    [Schema("uservoice")]
    public class ReleaseNoteMarker : IModel<int>
    {
        public int Id { get; set; }

        [Key]
        [MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// release notes added after this timestamp are visible to this user's changelog
        /// </summary>
        public DateTime VisibleAfter { get; set; }
    }
}
