using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Areas.Identity.Data;
using PixelPerspective.Models;
using PixelPerspective.Data;

namespace PixelPerspective.Pages
{
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<PixelPerspectiveUser> _userManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly PixelPerspectiveContext _context;

        public UserProfileModel(ILogger<IndexModel> logger, UserManager<PixelPerspectiveUser> userManager, PixelPerspectiveContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public PixelPerspectiveUser user { get; set; }

        public PixelPerspectiveUser currentuser { get; set; }
        public List<Game> GameLibrary { get; set; }

        public async Task<IActionResult> OnGetAsync(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                return NotFound();
            }

            currentuser = await _userManager.GetUserAsync(User);

            user = await _userManager.Users.FirstOrDefaultAsync(u => u.DisplayName == displayName);
            
            if (user == null)
            {
                RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
