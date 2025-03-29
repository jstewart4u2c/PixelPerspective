using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IGDB.Models;
using Microsoft.AspNetCore.Identity;
using PixelPerspective.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace PixelPerspective.Pages
{
    public class SearchResultsModel : PageModel
    {
        private readonly IGDBService _igdbService;
        private readonly UserManager<PixelPerspectiveUser> _userManager;

        public Game[] SearchResults { get; set; } = Array.Empty<Game>();
        public List<PixelPerspectiveUser> UserResults { get; set; } = new List<PixelPerspectiveUser>();

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SearchType { get; set; } = "games";


        public SearchResultsModel(IGDBService igdbService, UserManager<PixelPerspectiveUser> userManager)
        {
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
        public async Task<IActionResult> OnGetAsync(string searchQuery, string searchType)
        {
            SearchQuery = searchQuery;
            SearchType = searchType;

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
                    .Where(u => u.DisplayName.ToLower().Contains(SearchQuery.ToLower()))
                    .Take(5)
                    .ToListAsync(); 
            }

            return Page(); 
        }
    }
}
