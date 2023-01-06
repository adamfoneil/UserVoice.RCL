using AO.Models;
using AO.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace UserVoice.Database
{
    [Schema("uservoice")]
    [UniqueConstraint(nameof(ExternalId))]
    public class ExternalItem : IModel<int>
    {
        public int Id { get; set; }

        [Key]
        [References(typeof(Item), CascadeDelete = true)]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(255)]        
        public string Url { get; set; }

        public int ExternalId { get; set; }
    }
}
