using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PixelPerspective.Data;
using PixelPerspective.Areas.Identity.Data;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PixelPerspectiveContextConnection") ?? throw new InvalidOperationException("Connection string 'PixelPerspectiveContextConnection' not found.");

builder.Services.AddDbContext<PixelPerspectiveContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<PixelPerspectiveUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<PixelPerspectiveContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IGDBService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "titles",
    pattern: "titles/{id:int}",
    defaults: new { controller = "Titles", action = "Index" }
);

app.Run();
