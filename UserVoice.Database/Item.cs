using AO.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public enum ItemType
    {
        /// <summary>
        /// documents anything that hurts my productivity as a user (bug, deficiency), can be voted on
        /// </summary>
        Issue = 1,
        /// <summary>
        /// describes some new functionality I'd like to see that can be voted on by the community
        /// </summary>
        FeatureIdea = 2,
        /// <summary>
        /// describes functionality seeking end-user sign off as part of a release
        /// </summary>
        TestCase = 3,
        /// <summary>
        /// describes newly-released functionality
        /// </summary>
        ReleaseNote = 4
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

        [NotMapped]
        public int? AssignToUserId { get; set; }

        [NotMapped]
        public int? ExternalId { get; set; }

        [NotMapped]
        public string ExternalUrl { get; set; }

        public static Dictionary<Role, IEnumerable<ItemType>> AllowedTypes => new Dictionary<Role, IEnumerable<ItemType>>()
        {
            [Role.ProductOwner] = new[] { ItemType.FeatureIdea, ItemType.TestCase, ItemType.Issue, ItemType.ReleaseNote },
            [Role.User] = new[] { ItemType.FeatureIdea, ItemType.Issue },
            [Role.SignOffUser] = new[] { ItemType.FeatureIdea, ItemType.Issue, ItemType.TestCase }
        };

        public static IEnumerable<ItemType> GetAllowedTypes(Role roleFlags) => 
            AllowedTypes
                .SelectMany(kp => kp.Value, (kp, type) => new { Role = kp.Key, Type = type })
                .Where(item => roleFlags.HasFlag(item.Role))
                .Select(item => item.Type)
                .Distinct();
    }
}