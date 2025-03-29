// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Areas.Identity.Data;

namespace PixelPerspective.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<PixelPerspectiveUser> _userManager;
        private readonly SignInManager<PixelPerspectiveUser> _signInManager;

        public IndexModel(
            IWebHostEnvironment env,
            UserManager<PixelPerspectiveUser> userManager,
            SignInManager<PixelPerspectiveUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        public PixelPerspectiveUser user { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public string ProfileImagePath { get; set; }

        [BindProperty]
        public IFormFile ProfileImage { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Bio")]
            [StringLength(255, ErrorMessage = "Bio cannot exceed 255 characters.")]
            public string Bio { get; set; } = "This user has no bio";

            [Display(Name = "Display Name")]
            [Required(ErrorMessage = "Display Name is required.")]
            [StringLength(50, ErrorMessage = "Display Name cannot exceed 50 characters.")]
            public string DisplayName { get; set; } = string.Empty;

        }

        private async Task LoadAsync(PixelPerspectiveUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var bio = user.Bio;
            var displayName = user.DisplayName;

            Username = userName;
            

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Bio = user.Bio,
                DisplayName = displayName,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteImageAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Unable to load user");
            }

            if (!string.IsNullOrEmpty(user.ProfileImagePath))
            {
                var imagePath = Path.Combine(_env.WebRootPath, "ProfileImages", user.ProfileImagePath);

                // Deletes image in root folder
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // update database column to null for current user
                user.ProfileImagePath = null;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                await OnPostDeleteImageAsync();

                var uploadPath = Path.Combine(_env.WebRootPath, "ProfileImages");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // create unique file name
                var uniqueName = Guid.NewGuid().ToString() + "_" + ProfileImage.FileName;
                var filePath = Path.Combine(uploadPath, uniqueName);

                using (var fStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(fStream);
                }

                // update user's profile image path in db
                user.ProfileImagePath = uniqueName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.Bio != user.Bio)
            {
                user.Bio = Input.Bio;
                await _userManager.UpdateAsync(user);
            }

            if (Input.DisplayName != user.DisplayName)
            {
                var existingDisplayName = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.DisplayName == Input.DisplayName);

                if (existingDisplayName != null)
                {
                    ModelState.AddModelError("Input.DisplayName", "This display name is already taken.");
                    return Page();
                }

                user.DisplayName = Input.DisplayName;
                await _userManager.UpdateAsync(user);
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
