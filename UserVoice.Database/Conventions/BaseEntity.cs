using System;
using System.ComponentModel.DataAnnotations;

namespace UserVoice.Database.Conventions
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        [Required]
        public string CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }

        [MaxLength(50)]
        public string ModifiedBy { get; set; }
    }
}
