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

    public DbSet<Game> Game { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;

    public DbSet<Friend> Friends { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PixelPerspectiveUser>(entity =>
        {
        });

        builder.Entity<Friend>(b =>
        {
            b.HasKey(x => new { x.UserId, x.UserFriendId });

            b.HasOne(x => x.User)
                .WithMany(x => x.Friends)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.UserFriend)
                .WithMany(x => x.FriendsOf)
                .HasForeignKey(x => x.UserFriendId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }

}
