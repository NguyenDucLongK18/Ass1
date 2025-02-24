using Ass1.Models;
using Ass1.Repositories;
using Ass1.Services;
using Ass1.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/";
        options.AccessDeniedPath = "/Home/";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = false;
    });

builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options
    .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("FUNewsManagementDB")));

builder.Services.AddSingleton<MailUtils>();

builder.Services.AddScoped<SystemAccountRepository>();
builder.Services.AddScoped<UserManagerService>();
builder.Services.AddScoped<NewsArticleRepository>();
builder.Services.AddScoped<NewsArticleService>();
builder.Services.AddScoped<TagRepository>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<CategoryService>();
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
