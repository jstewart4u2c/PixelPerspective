using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Data;
using PixelPerspective.Models;

namespace PixelPerspective.Pages.Titles
{
    public class AllReviewsModel : PageModel
    {
        private readonly PixelPerspectiveContext _context;

        public AllReviewsModel(PixelPerspectiveContext context)
        {
            _context = context;
        }

        public Game Game { get; set; } = default!;
        public IList<Review> Reviews { get; set; } = default!;
        public string SortOrder { get; set; } = "newest";

        public async Task<IActionResult> OnGetAsync(int id, string sort = "newest")
        {
            SortOrder = sort;
            Game = await _context.Game.FirstOrDefaultAsync(g => g.Id == id);
            if (Game == null) return NotFound();

            var reviewsQuery = _context.Reviews
                .Where(r => r.GameId == id && r.IsApproved)
                .Include(r => r.User);

            Reviews = sort switch
            {
                "rating_desc" => await reviewsQuery.OrderByDescending(r => r.Rating).ToListAsync(),
                "rating_asc" => await reviewsQuery.OrderBy(r => r.Rating).ToListAsync(),
                "likes" => await reviewsQuery.OrderByDescending(r => r.Likes - r.Dislikes).ToListAsync(),
                _ => await reviewsQuery.OrderByDescending(r => r.CreatedAt).ToListAsync()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostLikeAsync(int reviewId, int id)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return NotFound();

            var likeKey = $"LikedReview_{reviewId}";
            var dislikeKey = $"DislikedReview_{reviewId}";

            var liked = Request.Cookies.ContainsKey(likeKey);
            var disliked = Request.Cookies.ContainsKey(dislikeKey);

            if (liked)
            {
                review.Likes--;
                Response.Cookies.Delete(likeKey);
            }
            else
            {
                review.Likes++;
                Response.Cookies.Append(likeKey, "true");

                if (disliked)
                {
                    review.Dislikes--;
                    Response.Cookies.Delete(dislikeKey);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostDislikeAsync(int reviewId, int id)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return NotFound();

            var likeKey = $"LikedReview_{reviewId}";
            var dislikeKey = $"DislikedReview_{reviewId}";

            var liked = Request.Cookies.ContainsKey(likeKey);
            var disliked = Request.Cookies.ContainsKey(dislikeKey);

            if (disliked)
            {
                review.Dislikes--;
                Response.Cookies.Delete(dislikeKey);
            }
            else
            {
                review.Dislikes++;
                Response.Cookies.Append(dislikeKey, "true");

                if (liked)
                {
                    review.Likes--;
                    Response.Cookies.Delete(likeKey);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostReportAsync(int reviewId, int id)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                review.IsApproved = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { id });
        }
    }
}
