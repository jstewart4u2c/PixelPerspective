using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IGDB.Models;
using PixelPerspective.Models;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Data;
using Microsoft.AspNetCore.Identity;
using PixelPerspective.Areas.Identity.Data;

namespace PixelPerspective.Pages
{
    public class GameDetailsModel : PageModel
    {
        private readonly IGDBService _igdbService;
        private readonly PixelPerspectiveContext _context;
        private readonly UserManager<PixelPerspectiveUser> _userManager;

        public IGDB.Models.Game? Game { get; set; }

        public IList<Review> AllApprovedReviews { get; set; } = default!;
        public IList<Review> TopReviews { get; set; } = default!;

        [BindProperty]
        public string ReviewText { get; set; } = string.Empty;

        [BindProperty]
        public int UserReviewRating { get; set; } = 0;

        public List<GameLibrary> UserGameLibrary { get; set; } = new();

        public GameDetailsModel(IGDBService igdbService, PixelPerspectiveContext context, UserManager<PixelPerspectiveUser> userManager)
        {
            _igdbService = igdbService;
            _context = context;
            _userManager = userManager;
        }

        /*****
         * When game details page is fetched, get user's current library
         * to dynamically update the status of the add to library button.
         * 
         * Reviews for that game are also fetched for display. 
         *****/
        public async Task<IActionResult> OnGetAsync(long id)
        {
            Game = await _igdbService.GetGameByIdAsync(id);

            if (Game == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                UserGameLibrary = await _context.UserGameLibrary
                    .Where(g => g.UserId == user.Id)
                    .ToListAsync();
            }

            AllApprovedReviews = await _context.Reviews
                .Where(r => r.GameId == id && r.IsApproved)
                .Include(r => r.User)
                .ToListAsync();

            TopReviews = AllApprovedReviews
                .OrderByDescending(r => r.Likes - r.Dislikes)
                .Take(5)
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Game = await _igdbService.GetGameByIdAsync(id);

            if (Game == null)
            {
                return NotFound();
            }

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var existingGame = await _context.Game.FirstOrDefaultAsync(g => g.Id == Game.Id);

            if (existingGame == null)
            {
                // Insert the game record if it does not exist
                _context.Game.Add(new PixelPerspective.Models.Game
                {
                    Id = (int)Game.Id,
                    Title = Game.Name,
                });
                await _context.SaveChangesAsync();
            }

            // Create and add a new review object
            var review = new Review
            {
                GameId = (int)Game.Id,
                UserId = currentUser?.Id,
                ReviewText = ReviewText,
                Rating = UserReviewRating,
                CreatedAt = DateTime.UtcNow,
                Likes = 0,
                Dislikes = 0,
                IsApproved = true
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = id });
        }
    }
}
