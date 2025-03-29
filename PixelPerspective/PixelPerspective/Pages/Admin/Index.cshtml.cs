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

        public IndexModel(PixelPerspectiveContext context)
        {
            _context = context;
        }

        public IList<PixelPerspectiveUser> Users { get; set; } = default!;
        public IList<Game> Games { get; set; } = new List<Game>();
        public IList<Review> Reviews { get; set; } = new List<Review>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch users from the database
            Users = await _context.Users.ToListAsync();
            Games = await _context.Game.ToListAsync();
            Reviews = await _context.Reviews.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Fetch users from the database to ensure we have the latest data
            Users = await _context.Users.ToListAsync();
            Games = await _context.Game.ToListAsync();
            Reviews = await _context.Reviews.ToListAsync();

            // Loop through all the users and update their IsAdmin status
            foreach (var user in Users)
            {
                // Check if the form has the checkbox for this user
                var isAdminKey = $"IsAdmin_{user.Id}";
                var isAdminValue = Request.Form[isAdminKey];

                // Update the user's IsAdmin status based on whether the checkbox is checked
                user.IsAdmin = !string.IsNullOrEmpty(isAdminValue);  // If checked, set to true; if unchecked, set to false
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // After saving, reload the page to reflect the changes
            return RedirectToPage();
        }

        // Handle the deletion of a user with query string
        public async Task<IActionResult> OnGetDeleteUserAsync(string id)
        {
            // Ensure the id is a string (your UserId is a string in AspNetUsers)
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                // Remove the user from the database
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();  // Save the changes
            }

            // Redirect to the same page to refresh the user list
            return RedirectToPage();
        }
    }
}
