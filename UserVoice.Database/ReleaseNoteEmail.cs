using AO.Models;
using AO.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace UserVoice.Database
{
    /// <summary>
    /// tracks emails sent related to a release note, preventing duplicate messages
    /// </summary>
    [Schema("uservoice")]
    public class ReleaseNoteEmail : IModel<int>
    {
        public int Id { get; set; }

        [Key]
        [References(typeof(Item))]
        public int ItemId { get; set; }

        [Key]
        [References(typeof(User))]
        public int UserId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
