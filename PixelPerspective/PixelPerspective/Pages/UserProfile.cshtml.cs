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
        private readonly IGDBService _igdbService;

        public UserProfileModel(ILogger<IndexModel> logger, UserManager<PixelPerspectiveUser> userManager, PixelPerspectiveContext context, IGDBService igdbService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _igdbService = igdbService;
        }

        public PixelPerspectiveUser user { get; set; }

        public PixelPerspectiveUser currentuser { get; set; }
        public List<GameLibrary> GameLibrary { get; set; }
        public List<Review> Reviews { get; set; }

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

            GameLibrary = await _context.UserGameLibrary
                .Where(g => g.UserId == user.Id)
                .ToListAsync();

            Reviews = await _context.Reviews
                .Where(r => r.UserId == currentuser.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Page();
        }

    }
}
