using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IGDB.Models;
using Microsoft.AspNetCore.Identity;
using PixelPerspective.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Data;
using PixelPerspective.Models;
using System.Dynamic;

namespace PixelPerspective.Pages
{
    public class SearchResultsModel : PageModel
    {
        private readonly PixelPerspectiveContext _context;
        private readonly IGDBService _igdbService;
        private readonly UserManager<PixelPerspectiveUser> _userManager;

        public IGDB.Models.Game[] SearchResults { get; set; } = Array.Empty<IGDB.Models.Game>();
        public List<PixelPerspectiveUser> UserResults { get; set; } = new List<PixelPerspectiveUser>();

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SearchType { get; set; } = "games";

        public List<GameLibrary> UserGameLibrary { get; set; } = new();


        public SearchResultsModel(PixelPerspectiveContext context, IGDBService igdbService, UserManager<PixelPerspectiveUser> userManager)
        {
            _context = context;
            _igdbService = igdbService;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                ModelState.AddModelError(string.Empty, "Please enter a search");
                return Page();
            }

            if (SearchType == "games")
            {
                SearchResults = await _igdbService.SearchGamesAsync(SearchQuery);
            }
            else if (SearchType == "users")
            {
                UserResults = await _userManager.Users
                    .Where(u => u.DisplayName.Contains(SearchQuery) || u.Email.Contains(SearchQuery))
                    .ToListAsync();
            }

            return RedirectToPage("/SearchResults", new { SearchQuery = SearchQuery, SearchType = SearchType });
        }

        /****
         * OnGet searches the IGDB API for the searchQuery parameter, and searchType is assigned
         * when a user clicks the user or game button on the search results page. 
         * **/
        public async Task<IActionResult> OnGetAsync(string searchQuery, string searchType)
        {
            SearchQuery = searchQuery;
            SearchType = searchType;

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                UserGameLibrary = await _context.UserGameLibrary
                    .Where(g => g.UserId == user.Id)
                    .ToListAsync();
            }

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                return Page();
            }

            if (SearchType == "games")
            {
                // Search for games via IGDB API
                SearchResults = await _igdbService.SearchGamesAsync(SearchQuery);

            }
            else if (SearchType == "users")
            {
                // Search for users
                UserResults = await _userManager.Users
                    .Where(u => u.DisplayName.ToLower().Contains(SearchQuery.ToLower()) || u.Email.ToLower().Contains(SearchQuery.ToLower()))
                    .Take(5)
                    .ToListAsync(); 
            }

            return Page(); 
        }


        /****
         * AddToLibrary method first gets the user and checks to see if the game is already in their library.
         * If not, it retrieves the game information and adds the game to that user library. 
         */
        public async Task<IActionResult> OnPostAddToLibraryAsync(long igdbGameId, string gameTitle, string coverUrl, string searchQuery, string searchType)
        {
            var user = await _userManager.GetUserAsync(User);

            var existing = await _context.UserGameLibrary
                .FirstOrDefaultAsync(g => g.UserId == user.Id && g.IGDBGameId == igdbGameId);

            if (existing == null)
            {
                var newGame = new Models.GameLibrary
                {
                    UserId = user.Id,
                    IGDBGameId = igdbGameId,
                    GameTitle = gameTitle,
                    CoverUrl = coverUrl
                };

                _context.UserGameLibrary.Add(newGame);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/SearchResults", new { searchQuery = searchQuery, searchType = searchType });
        }


        /****
         * RemoveFromLibrary checks to make sure the game exists in the user library before removing it when 
         * the remove button is clicked.
         */
        public async Task<IActionResult> OnPostRemoveFromLibraryAsync(long igdbGameId, string gameTitle, string coverUrl, string searchQuery, string searchType)
        {
            var user = await _userManager.GetUserAsync(User);

            var existing = await _context.UserGameLibrary
                .FirstOrDefaultAsync(g => g.UserId == user.Id && g.IGDBGameId == igdbGameId);

            if (existing != null)
            {
                _context.UserGameLibrary.Remove(existing);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/SearchResults", new { searchQuery = searchQuery, searchType = searchType });
        }

    }
}

