using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        public IList<Review> AllApprovedReviews { get; set; } = default!;

        public string NoReviewsMessage { get; set; } = string.Empty;

        [BindProperty]
        public string ReviewText { get; set; } = string.Empty;

        [BindProperty]
        public int UserReviewRating { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Game = await _context.Game
                .FirstOrDefaultAsync(g => g.Id == id);

            if (Game == null)
            {
                return NotFound();
            }

            AllApprovedReviews = await _context.Reviews
                .Where(r => r.GameId == id && r.IsApproved)
                .Include(r => r.User)
                .ToListAsync();

            TopReviews = AllApprovedReviews
                .OrderByDescending(r => r.Likes - r.Dislikes)
                .Take(5)
                .ToList();

            if (!TopReviews.Any())
            {
                NoReviewsMessage = "This game does not have any reviews yet!";
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            Game = await _context.Game
                .FirstOrDefaultAsync(g => g.Id == id);

            if (Game == null)
            {
                return NotFound();
            }

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var review = new Review
            {
                GameId = id,
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

        [BindProperty]
        public int ReviewId { get; set; }

        public async Task<IActionResult> OnPostLikeAsync(int id, int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return NotFound();

            var cookieKeyLike = $"LikedReview_{reviewId}";
            var cookieKeyDislike = $"DislikedReview_{reviewId}";

            var liked = Request.Cookies.ContainsKey(cookieKeyLike);
            var disliked = Request.Cookies.ContainsKey(cookieKeyDislike);

            if (liked)
            {
                review.Likes--;
                Response.Cookies.Delete(cookieKeyLike);
            }
            else
            {
                review.Likes++;
                Response.Cookies.Append(cookieKeyLike, "true");

                if (disliked)
                {
                    review.Dislikes--;
                    Response.Cookies.Delete(cookieKeyDislike);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = id });
        }


        public async Task<IActionResult> OnPostDislikeAsync(int id, int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return NotFound();

            var cookieKeyLike = $"LikedReview_{reviewId}";
            var cookieKeyDislike = $"DislikedReview_{reviewId}";

            var liked = Request.Cookies.ContainsKey(cookieKeyLike);
            var disliked = Request.Cookies.ContainsKey(cookieKeyDislike);

            if (disliked)
            {
                review.Dislikes--;
                Response.Cookies.Delete(cookieKeyDislike);
            }
            else
            {
                review.Dislikes++;
                Response.Cookies.Append(cookieKeyDislike, "true");

                if (liked)
                {
                    review.Likes--;
                    Response.Cookies.Delete(cookieKeyLike);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = id });
        }



        public async Task<IActionResult> OnPostReportAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(ReviewId);
            if (review != null)
            {
                review.IsApproved = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { id = id });
        }
    }
}
