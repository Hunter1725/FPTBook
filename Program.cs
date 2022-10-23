using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using FPTBook.Utils;
using Microsoft.AspNetCore.Identity;
using FPTBook.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FPTBookIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FPTBookIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'FPTBookIdentityDbContextConnection' not found.")));

builder.Services.AddDefaultIdentity<BookUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FPTBookIdentityDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".BookTicket.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(300);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope()){
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<BookUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await ContextSeed.SeedRolesAsync(userManager, roleManager);
    await ContextSeed.SeedSuperAdminAsync(userManager, roleManager);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    
app.MapRazorPages();
app.Run();
