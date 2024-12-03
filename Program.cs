using Lemoo_pos.Data;
using Lemoo_pos.Services.Interfaces;
using Lemoo_pos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Lemoo_pos.Helper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Nest;


DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);


//cloudinary config
var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_NAME"); ;
var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

var account = new Account(cloudName, apiKey, apiSecret);
var cloudinary = new Cloudinary(account);


// database configurations

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");


// config jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("BASE_URL"),
        ValidAudience = Environment.GetEnvironmentVariable("BASE_URL"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SERCRET")))
    };
});


builder.Services.AddAuthorization();


// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddSingleton(cloudinary);

builder.Services.AddDbContext<AppDbContext>(options =>
    options
    .UseNpgsql(connectionString)
    .EnableSensitiveDataLogging()
     .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
  );



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


builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All); 
    });


builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var settings = new ConnectionSettings(new Uri(Environment.GetEnvironmentVariable("ELASTICSEARCH_URL")))
    .ApiKeyAuthentication(
        apiKey: Environment.GetEnvironmentVariable("ELASTICSEARCH_API_KEY"),
        id: Environment.GetEnvironmentVariable("ELASTICSEACRCH_CLIENT_ID")
    ).DefaultIndex("products"); 

    var client = new ElasticClient(settings);
    return client;
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
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IBrandService, BrandService>();
builder.Services.AddTransient<IBranchService, BranchService>();
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddTransient<IStaffService, StaffService>();
builder.Services.AddTransient<IElasticsearchService, ElasticsearchService>();
builder.Services.AddTransient<ISearchService, SearchService>();
builder.Services.AddTransient<IOrderService, OrderService>();

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

app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
});

app.Use(async (context, next) =>
{
    await next.Invoke();
});

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
