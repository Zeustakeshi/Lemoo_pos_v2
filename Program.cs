using Lemoo_pos.Data;
using Lemoo_pos.Services.Interfaces;
using Lemoo_pos.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Lemoo_pos.Helper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Nest;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Areas.Api.Services;
using Lemoo_pos.Areas.Api.Filters;
using MassTransit;
using Lemoo_pos.Consumers;


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

// config rabbitmq
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
             Environment.GetEnvironmentVariable("RABBITMQ_VHOST"),
               h =>
               {
                   h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USER"));
                   h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"));
               });
        cfg.ReceiveEndpoint("new_authority", e =>
        {
            e.ConfigureConsumer<CreateAuthorityConsumer>(context);
        });

        cfg.ReceiveEndpoint("new_authority_permission_batch", e =>
        {
            e.Batch<CreateAuthorityPermissionConsumer>(b =>
           {
               b.MessageLimit = 20;      // Số lượng tối đa message trong một batch
               b.TimeLimit = TimeSpan.FromSeconds(5); // Thời gian chờ tối đa để gom batch
           });
            e.ConfigureConsumer<CreateAuthorityPermissionConsumer>(context);
        });

        cfg.ReceiveEndpoint("update_product_brand", e =>
        {
            e.ConfigureConsumer<UpdateProductBrandConsumer>(context);
        });

        cfg.ReceiveEndpoint("save_product_search", e =>
        {
            e.ConfigureConsumer<SaveSearchProductConsumer>(context);
        });
        cfg.ReceiveEndpoint("save_customer_search", e =>
        {
            e.ConfigureConsumer<SaveSearchCustomerConsumer>(context);
        });
        cfg.ReceiveEndpoint("init-category", e =>
        {
            e.ConfigureConsumer<InitInventoryConsumer>(context);
        });
        cfg.ReceiveEndpoint("update-order-customer", e =>
        {
            e.ConfigureConsumer<UpdateOrderCustomerConsumer>(context);
        });

        cfg.ReceiveEndpoint("create_order_batch", e =>
        {
            e.Batch<CreateOrderBatchConsumer>(b =>
           {
               b.MessageLimit = 50;      // Số lượng tối đa message trong một batch
               b.TimeLimit = TimeSpan.FromSeconds(5); // Thời gian chờ tối đa để gom batch
           });
            e.ConfigureConsumer<CreateOrderBatchConsumer>(context);
        });

    });

    x.AddConsumer<CreateAuthorityConsumer>();
    x.AddConsumer<UpdateProductBrandConsumer>();
    x.AddConsumer<CreateAuthorityPermissionConsumer>();
    x.AddConsumer<SaveSearchProductConsumer>();
    x.AddConsumer<InitInventoryConsumer>();
    x.AddConsumer<SaveSearchCustomerConsumer>();
    x.AddConsumer<UpdateOrderCustomerConsumer>();
    x.AddConsumer<CreateOrderBatchConsumer>();
});


builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
    });

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
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
builder.Services.AddTransient<ICustomerService, CustomerService>();
// API
builder.Services.AddTransient<IAccountServiceApi, AccountServiceApi>();
builder.Services.AddTransient<IProductServiceApi, ProductServiceApi>();
builder.Services.AddTransient<IStaffServiceApi, StaffServiceApi>();
builder.Services.AddTransient<ICustomerServiceApi, CustomerServiceApi>();
builder.Services.AddTransient<IShiftServiceApi, ShiftServiceApi>();
builder.Services.AddTransient<IOrderServiceApi, OrderServiceApi>();
builder.Services.AddTransient<IBranchServiceApi, BranchServiceApi>();
builder.Services.AddTransient<IProductServiceApi, ProductServiceApi>();
builder.Services.AddTransient<ISearchServiceApi, SearchServiceApi>();

// HELPER
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
    //var headers = context.Request.Headers;
    //await Console.Out.WriteLineAsync();
    //await Console.Out.WriteLineAsync();
    //await Console.Out.WriteLineAsync("======================================================= || ======================================================");
    //foreach (var header in headers)
    //{
    //    await Console.Out.WriteLineAsync($"key={header.Key}: {header.Value}");
    //}
    //await Console.Out.WriteLineAsync("======================================================= || ======================================================");
    //await Console.Out.WriteLineAsync();
    //await Console.Out.WriteLineAsync();

    await next.Invoke();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=PosApi}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
