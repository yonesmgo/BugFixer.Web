using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.Common;
using BugFixer.IoC;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

#region services
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<ScoreManagementViewModel>(builder.Configuration.GetSection("ScoreManagement"));
#region DbContext
builder.Services.AddDbContext<BugFixerDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("BugFixerConnection")));
#endregion
#region RegisterDependency
DependencyContainers.RegisteDependency(builder.Services);
#endregion
#region ncode
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
#endregion
#region Authentication
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(option =>
{
    option.LoginPath = "/Login";
    option.LogoutPath = "/Logout";
    option.ExpireTimeSpan = TimeSpan.FromDays(30);
});
#endregion
#endregion
#region MiddleWare
var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();

#endregion
