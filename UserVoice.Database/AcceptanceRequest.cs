﻿using AO.Models;
using System.ComponentModel.DataAnnotations;
using UserVoice.Database.Conventions;

namespace UserVoice.Database
{
    public enum Response
    {
        /// <summary>
        /// no response yet
        /// </summary>
        Pending,
        /// <summary>
        /// item is accepted
        /// </summary>
        Accepted,
        /// <summary>
        /// item not accepted (please provide reason)
        /// </summary>
        Declined
    }

    /// <summary>
    /// associates an item with a user, representing a request for sign-off on a test item
    /// </summary>
    public class AcceptanceRequest : BaseEntity
    {
        [Key]
        [References(typeof(Item))]
        public int ItemId { get; set; }

        /// <summary>
        /// must be a user who allows acceptance testing
        /// </summary>
        [Key]
        [References(typeof(User))]
        public int UserId { get; set; }

        public Response Response { get; set; } = Response.Pending;

        public string Comments { get; set; }
    }
}
