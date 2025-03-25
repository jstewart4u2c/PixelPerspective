using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IGDB.Models;

namespace PixelPerspective.Pages
{
    public class SearchResultsModel : PageModel
    {
        private readonly IGDBService _igdbService;
        public Game[] SearchResults { get; set; } = Array.Empty<Game>();

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = string.Empty;

        public SearchResultsModel(IGDBService igdbService)
        {
            _igdbService = igdbService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                ModelState.AddModelError(string.Empty, "Please enter a search");
                return Page();
            }

            SearchResults = await _igdbService.SearchGamesAsync(SearchQuery);
            return Page();
        }

    }
}
