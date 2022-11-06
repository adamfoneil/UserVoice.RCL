using AO.Models;
using AO.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace UserVoice.Database.Conventions
{
    [Schema("uservoice")]
    public abstract class BaseEntity : IModel<int>
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        [Required]
        public string CreatedBy { get; set; } = "_notset"; // filled in by BaseRepository.BeforeSaveAsync

        public DateTime? DateModified { get; set; }

        [MaxLength(50)]
        public string ModifiedBy { get; set; }
    }
}
