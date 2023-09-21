using AO.Models;
using System.ComponentModel.DataAnnotations;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public class ItemPriority : BaseEntity
    {
        [Key]
        [References(typeof(Item))]
        public int ItemId { get; set; }

        public int Order { get; set; }
    }
}
