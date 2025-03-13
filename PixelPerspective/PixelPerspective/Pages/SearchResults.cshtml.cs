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
        public string SearchQuery { get; set; } = "";

        public SearchResultsModel(IGDBService igdbService)
        {
            _igdbService = igdbService;
        }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                SearchResults = await _igdbService.SearchGamesAsync(SearchQuery);
            }
        }
    }
}
