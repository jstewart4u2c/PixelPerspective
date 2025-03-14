using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Areas.Identity.Data;
using PixelPerspective.Models;

namespace PixelPerspective.Data;

public class PixelPerspectiveContext : IdentityDbContext<PixelPerspectiveUser>
{
    public PixelPerspectiveContext(DbContextOptions<PixelPerspectiveContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PixelPerspectiveUser>(entity =>
        {
        });
    }

    public DbSet<PixelPerspective.Models.Game> Game { get; set; } = default!;
}
