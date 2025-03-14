using Microsoft.AspNetCore.Mvc;
using PixelPerspective.Data;
using System.Linq;

namespace PixelPerspective.Controllers
{
    public class TitleController : Controller
    {
        private readonly PixelPerspectiveContext _context;

        public TitleController(PixelPerspectiveContext context)
        {
            _context = context;
        }

        [Route("title/{id}")]
        public IActionResult Index(int id)
        {
            var game = _context.Game.FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                return NotFound(); // Return a 404 if the game doesn't exist
            }

            return View(game);
        }
    }
}