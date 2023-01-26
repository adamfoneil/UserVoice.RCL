using AO.Models;
using AO.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace UserVoice.Database
{
    [Schema("uservoice")]
    public class ExternalItemSource : IModel<int>
    {
        public int Id { get; set; }

        [Key]
        [MaxLength(50)]
        public string Name { get; set; }

        public DateTime LastMerge { get; set; }
    }
}
