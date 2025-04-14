using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Areas.Identity.Data;
using PixelPerspective.Data;
using PixelPerspective.Models;

namespace PixelPerspective.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly PixelPerspectiveContext _context;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(PixelPerspectiveContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IList<PixelPerspectiveUser> Users { get; set; } = new List<PixelPerspectiveUser>();
        public IList<Game> Games { get; set; } = new List<Game>();
        public IList<Review> Reviews { get; set; } = new List<Review>();

        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public DateTime ReleaseDate { get; set; }
        [BindProperty] public string Genre { get; set; } = string.Empty;
        [BindProperty] public string Description { get; set; } = string.Empty;
        [BindProperty] public string TrailerPath { get; set; } = string.Empty;
        [BindProperty] public IFormFile ThumbnailFile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string tab = "users")
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (currentUser == null || currentUser.IsAdmin != true)
            {
                return Forbid();
            }
            Users = await _context.Users.ToListAsync();
            Games = await _context.Game.ToListAsync();
            Reviews = await _context.Reviews.ToListAsync();

            ViewData["CurrentTab"] = tab;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (currentUser == null || currentUser.IsAdmin != true)
            {
                return Forbid();
            }
            Users = await _context.Users.ToListAsync();
            foreach (var user in Users)
            {
                var isAdminKey = $"IsAdmin_{user.Id}";
                var isAdminValue = Request.Form[isAdminKey];
                user.IsAdmin = !string.IsNullOrEmpty(isAdminValue);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { tab = "users" });
        }

        public async Task<IActionResult> OnPostAddGameAsync()
        {
            if (!ModelState.IsValid || ThumbnailFile == null)
                return RedirectToPage(new { tab = "games" });

            var safeFileName = $"{Title}.jpg";
            var savePath = Path.Combine(_environment.WebRootPath, "Thumbnails", safeFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await ThumbnailFile.CopyToAsync(fileStream);
            }

            var game = new Game
            {
                Title = Title,
                ReleaseDate = ReleaseDate,
                Genre = Genre,
                Description = Description,
                ThumbnailImagePath = $"Thumbnails/{safeFileName}",
                TrailerPath = TrailerPath
            };

            _context.Game.Add(game);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { tab = "games" });
        }

        public async Task<IActionResult> OnGetDeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { tab = "users" });
        }

        public async Task<IActionResult> OnPostApproveReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                review.IsApproved = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { tab = "reviews" });
        }

        public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { tab = "reviews" });
        }
        public async Task<IActionResult> OnPostEditGameAsync(int gameId)
        {
            ViewData["CurrentTab"] = "games";
            ViewData["EditingGameId"] = gameId;

            Users = await _context.Users.ToListAsync();
            Games = await _context.Game.ToListAsync();
            Reviews = await _context.Reviews.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateGameAsync(int gameId)
        {
            var game = await _context.Game.FindAsync(gameId);
            if (game == null) return NotFound();

            game.Title = Request.Form["Title"];
            game.ReleaseDate = DateTime.Parse(Request.Form["ReleaseDate"]);
            game.Genre = Request.Form["Genre"];
            game.Description = Request.Form["Description"];
            game.TrailerPath = Request.Form["TrailerPath"];

            if (ThumbnailFile != null && ThumbnailFile.Length > 0)
            {
                var fileName = $"{game.Title.Replace(" ", "_")}.jpg";
                var filePath = Path.Combine("wwwroot", "Thumbnails", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ThumbnailFile.CopyToAsync(stream);
                }

                game.ThumbnailImagePath = $"Thumbnails/{fileName}";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { tab = "games" });
        }

        public async Task<IActionResult> OnPostDeleteGameAsync(int gameId)
        {
            var game = await _context.Game.FindAsync(gameId);
            if (game != null)
            {
                _context.Game.Remove(game);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { tab = "games" });
        }
    }
}
