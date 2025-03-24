using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IGDB.Models;
using Microsoft.AspNetCore.Identity;
using PixelPerspective.Data;
using Microsoft.EntityFrameworkCore;
using Elfie.Serialization;

namespace PixelPerspective.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PixelPerspectiveContext _context;

        public IndexModel(ILogger<IndexModel> logger, PixelPerspectiveContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<PixelPerspective.Models.Game> TopBar { get; set; } = new List<Models.Game>();
        public List<PixelPerspective.Models.Game> NewReleases { get; set; } = new List<Models.Game>();
        public List<PixelPerspective.Models.Game> GamesOfTheYear { get; set; } = new List<Models.Game>();
        public List<PixelPerspective.Models.Game> TrendingGames { get; set; } = new List<Models.Game>();
        public List<PixelPerspective.Models.Game> OurTopPicks { get; set; } = new List<Models.Game>();

        public List<String> ThumbnailPaths { get; set; } = new List<String>();


        public async Task OnGetASync()
        {
            //implementing random element
            var rnd = new Random();

            //Get Top Bar games
            TopBar = await _context.Game
                .OrderBy(x => rnd.Next())
                .Take(8)
                .ToListAsync();

            //Get New Releases
            NewReleases = await _context.Game
                .Take(5)
                .ToListAsync();

            //Get Game of the Year nominees
            GamesOfTheYear = await _context.Game
                .Take(5)
                .ToListAsync();

            //Get trending games
            TrendingGames = await _context.Game
                .Take(5)
                .ToListAsync();

            //Get our top picks
            OurTopPicks = await _context.Game
                .Take(5)
                .ToListAsync();
        }
    }
}
