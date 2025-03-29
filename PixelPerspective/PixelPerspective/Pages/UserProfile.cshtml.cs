using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Areas.Identity.Data;
using PixelPerspective.Models;

namespace PixelPerspective.Pages
{
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<PixelPerspectiveUser> _userManager;

        public UserProfileModel(UserManager<PixelPerspectiveUser> userManager)
        {
            _userManager = userManager;
        }

        public PixelPerspectiveUser user { get; set; }
        public List<Game> GameLibrary { get; set; }

        public async Task<IActionResult> OnGetAsync(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                return NotFound();
            }

            user = await _userManager.Users.FirstOrDefaultAsync(u => u.DisplayName == displayName);
            
            if (user == null)
            {
                RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
