using Lemoo_pos.Data;
using Lemoo_pos.Services.Interfaces;
using Lemoo_pos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Lemoo_pos.Helper;

var builder = WebApplication.CreateBuilder(args);


// database configurations
var connectionString = builder.Configuration.GetConnectionString("Postgresql");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "SessionApp:";
});


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Liên kết session với Redis
    options.IOTimeout = TimeSpan.FromSeconds(1);
});


builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IStoreService, StoreService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IAuthorityService, AuthorityService>();

builder.Services.AddSingleton<PasswordHelper>();

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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
