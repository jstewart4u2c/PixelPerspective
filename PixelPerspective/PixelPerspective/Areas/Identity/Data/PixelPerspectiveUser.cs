using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PixelPerspective.Areas.Identity.Data;

// Add profile data for application users by adding properties to the PixelPerspectiveUser class
public class PixelPerspectiveUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "Date")]
    public DateTime? Birthday { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(255)")]
    public string? ProfileImagePath {  get; set; }

    // Site Specific Data
    public int ReputationPoints { get; set; } = 0;
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

}

