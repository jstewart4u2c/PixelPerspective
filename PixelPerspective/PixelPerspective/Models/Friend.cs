using System.ComponentModel.DataAnnotations;
using PixelPerspective.Areas.Identity.Data;

namespace PixelPerspective.Models
{
    public class Friend
    {
        public string UserId { get; set; } // Foreign key to aspnetusers
        public PixelPerspectiveUser User { get; set; } 

        public string UserFriendId { get; set; } // Foreign key to aspnetusers

        public PixelPerspectiveUser UserFriend { get; set; } 
        
    }
}
