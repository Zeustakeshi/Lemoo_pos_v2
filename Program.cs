using Lemoo_pos.Data;
using Lemoo_pos.Services.Interfaces;
using Lemoo_pos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Lemoo_pos.Helper;
using CloudinaryDotNet;

var builder = WebApplication.CreateBuilder(args);


//cloudinary config
var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var cloudName = cloudinarySettings["CloudName"];
var apiKey = cloudinarySettings["ApiKey"];
var apiSecret = cloudinarySettings["ApiSecret"];


var account = new Account(cloudName, apiKey, apiSecret);
var cloudinary = new Cloudinary(account);


// database configurations
var connectionString = builder.Configuration.GetConnectionString("Postgresql");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");



// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(cloudinary);

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
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IAuthorityService, AuthorityService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IOtpService, OtpService>();
builder.Services.AddTransient<IProductCategoryService, ProductCategoryService>();
builder.Services.AddTransient<ICloudinaryService, CloudinaryService>();

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
