using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Areas.Identity.Data;
using PixelPerspective.Data;
using PixelPerspective.Models;

namespace PixelPerspective.Pages.Titles
{
    public class IndexModel : PageModel
    {
        private readonly PixelPerspectiveContext _context;

        public IndexModel(PixelPerspectiveContext context)
        {
            _context = context;
        }

        public Game Game { get; set; } = default!;
        public IList<Review> TopReviews { get; set; } = default!;
        public IList<PixelPerspectiveUser> Users { get; set; } = default!;
        public string NoReviewsMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Game = await _context.Game
                .FirstOrDefaultAsync(g => g.Id == id);

            if (Game == null)
            {
                return NotFound();
            }

            TopReviews = await _context.Reviews
                .Where(r => r.GameId == id && r.IsApproved)
                .OrderByDescending(r => r.Likes)
                .Take(5)
                .Include(r => r.User)
                .ToListAsync();

            if (!TopReviews.Any())
            {
                NoReviewsMessage = "This game does not have any reviews yet!";
            }

            return Page();
        }

    }
}
