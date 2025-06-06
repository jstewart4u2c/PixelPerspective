﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Data;
using PixelPerspective.Models;

namespace PixelPerspective.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly PixelPerspective.Data.PixelPerspectiveContext _context;

        public IndexModel(PixelPerspective.Data.PixelPerspectiveContext context)
        {
            _context = context;
        }

        public IList<Game> Game { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Game = await _context.Game.ToListAsync();
        }
    }
}
