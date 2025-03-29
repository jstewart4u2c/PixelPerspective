using PixelPerspective.Areas.Identity.Data;
using System;

namespace PixelPerspective.Models
{
    public class Review
    {
        public int Id { get; set; }          // Primary key
        public string UserId { get; set; }   // Foreign Key to AspNetUsers
        public int GameId { get; set; }      // Foreign Key to Game
        public string ReviewText { get; set; } // The actual review content
        public int Rating { get; set; }      // Rating on a scale of 1-5
        public DateTime CreatedAt { get; set; } // When the review was created
        public int Likes { get; set; }        // Likes count for the review
        public int Dislikes { get; set; }     // Dislikes count for the review
        public bool IsApproved { get; set; }  // Admin approval status for the review
        public PixelPerspectiveUser User { get; set; }
    }
}