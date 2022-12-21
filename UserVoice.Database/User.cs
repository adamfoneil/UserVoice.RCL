using AO.Models;
using AO.Models.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    [Flags]
    public enum Role
    {
        /// <summary>
        /// can make requests, vote on features, post comments (bot not select status)
        /// </summary>
        User = 1,
        /// <summary>
        /// developer or product rep who can post comments with a status selected,
        /// can add work item links
        /// </summary>
        ProductOwner = 2,
        /// <summary>
        /// may submit test items and reply to acceptance requests
        /// </summary>
        SignOffUser = 4
    }

    /// <summary>
    /// users will be imported from the host application and periodically synced
    /// </summary>    
    [UniqueConstraint(nameof(Email))]
    public class User : BaseEntity
    {
        [Key]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        public Role Role { get; set; }

        [MaxLength(50)]
        public string TimeZoneId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime LocalTime => Timestamp.Local(TimeZoneId);

        public bool MayAssignAcceptanceRequests => Role.HasFlag(Role.ProductOwner) || Role.HasFlag(Role.SignOffUser);

#if NET6_0_OR_GREATER
        public override bool Equals(object? obj)
        {
            if (obj is User u)
            {
                return Email?.ToLower().Equals(u.Email.ToLower()) ?? false;
            }

            return false;
        }

        public override int GetHashCode() => Email?.ToLower().GetHashCode() ?? 0;        
#endif
    }
}
