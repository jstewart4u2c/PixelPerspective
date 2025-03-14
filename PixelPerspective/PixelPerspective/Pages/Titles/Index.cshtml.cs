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

        public PixelPerspective.Models.Game? Game { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Game = await _context.Game.FirstOrDefaultAsync(g => g.Id == id);

            if (Game == null)
            {
                return NotFound();
            }

            Console.WriteLine($"ThumbnailImagePath: {Game.ThumbnailImagePath}");

            return Page();
        }
    }
}
