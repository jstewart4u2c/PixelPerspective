using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Areas.Identity.Data;
using PixelPerspective.Models;
using PixelPerspective.Data;

namespace PixelPerspective.Pages
{
    public class FriendsListModel : PageModel
    {
		private readonly UserManager<PixelPerspectiveUser> _userManager;
		private readonly ILogger<IndexModel> _logger;
		private readonly PixelPerspectiveContext _context;

		public FriendsListModel(ILogger<IndexModel> logger, UserManager<PixelPerspectiveUser> userManager, PixelPerspectiveContext context)
		{
			_logger = logger;
			_userManager = userManager;
			_context = context;
		}

		public PixelPerspectiveUser user { get; set; }
		public List<PixelPerspectiveUser> users { get; set; }

		public async Task<IActionResult> OnGetAsync(string displayName)
		{
			if (string.IsNullOrEmpty(displayName))
			{
				return NotFound();
			}

			user = await _userManager.Users.FirstOrDefaultAsync(u => u.DisplayName == displayName);

			if (user == null)
			{
				RedirectToPage("/Index");
			}

			users = await _userManager.Users.ToListAsync();

			return Page();
		}
	}
}
