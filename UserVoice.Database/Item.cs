using AO.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public enum ItemType
    {
        /// <summary>
        /// documents anything that hurts my productivity as a user (bug, deficiency), can be voted on
        /// </summary>
        Impediment = 1,
        /// <summary>
        /// describes some new functionality I'd like to see that can be voted on by the community
        /// </summary>
        Feature = 2,
        /// <summary>
        /// describes functionality seeking end-user sign off as part of a release
        /// </summary>
        TestCase = 3
    }

    public class Item : BaseEntity
    {
        [Required]
        public ItemType Type { get; set; }

        [Required]
        [MaxLength(255)]        
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [References(typeof(Comment))]
        public int? StatusCommentId { get; set; }

        /// <summary>
        /// true means the item is visible to everyone.
        /// false would mean several things:
        /// - an acceptance request not ready for voting
        /// - feedback acted upon, a feature implemented, and therefore not open for voting
        /// </summary>
        public bool IsActive { get; set; } = true;

        public static Dictionary<Role, IEnumerable<ItemType>> AllowedTypes => new Dictionary<Role, IEnumerable<ItemType>>()
        {
            [Role.ProductOwner] = new[] { ItemType.Feature, ItemType.TestCase, ItemType.Impediment },            
            [Role.User] = new[] { ItemType.Feature, ItemType.Impediment },
            [Role.SignOffUser] = new[] { ItemType.Feature, ItemType.Impediment, ItemType.TestCase }
        };        
    }
}