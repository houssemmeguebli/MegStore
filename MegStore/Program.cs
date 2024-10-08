using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MegStore.Core.Entities.Users;
using MegStore.Infrastructure.Data;
using MegStore.Infrastructure.Repositories;
using MegStore.Application.Services;
using MegStore.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.RateLimiting;
using MegStore.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext for Entity Framework Core
builder.Services.AddDbContext<MegStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<MegStoreContext>()
    .AddDefaultTokenProviders();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register application services
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Load EmailSettings from configuration
var emailSettings = builder.Configuration.GetSection("EmailSettings");
if (string.IsNullOrWhiteSpace(emailSettings["Username"]) || string.IsNullOrWhiteSpace(emailSettings["Password"]))
{
    throw new Exception("Email settings are not configured properly.");
}

// Add EmailService to the service collection
builder.Services.AddSingleton<IEmailService>(new EmailService(
    emailSettings["Username"],
    emailSettings["Password"]
));

// AutoMapper configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure JSON serialization options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization options to ignore cycles and references
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 64; // Adjust depth as needed
    });

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // JWT Bearer Authorization
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("AdminOrSuperAdmin", policy => policy.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer")); // Adjust as needed
});

// Configure FormOptions (e.g., file upload size limit)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB limit
});

// Configure Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(10)
            }));
});

// Build the app
var app = builder.Build();

// Ensure database is migrated and create SuperAdmin if not present
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();

    await CreateSuperAdmin(userManager, roleManager);
}

// Function to create SuperAdmin user
async Task CreateSuperAdmin(UserManager<User> userManager, RoleManager<IdentityRole<long>> roleManager)
{
    // Define SuperAdmin role
    var superAdminRole = "SuperAdmin";

    // Create SuperAdmin role if it doesn't exist
    if (!await roleManager.RoleExistsAsync(superAdminRole))
    {
        await roleManager.CreateAsync(new IdentityRole<long>(superAdminRole));
    }

    var superAdminEmail = "superadmin@megStore.com";
    if (await userManager.FindByEmailAsync(superAdminEmail) == null)
    {
        // Create a new SuperAdmin user
        var superAdmin = new User
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            fullName = "superadmin",
            PhoneNumber = "string",
            address = "address",
            gender = Gender.Male,
            dateOfbirth = DateTime.Now,
            dateOfCreation = DateTime.Now,
        };

        // Set a default password for the SuperAdmin
        var password = "SuperAdmin123!";

        // Create the user with the specified password
        var result = await userManager.CreateAsync(superAdmin, password);

        // Assign the SuperAdmin role to the user
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(superAdmin, superAdminRole);
        }
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // Serve static files before enabling CORS and Authentication/Authorization

// Apply CORS policy
app.UseCors("AllowAllOrigins");

// Add rate limiting middleware
app.UseRateLimiter();

app.UseAuthentication(); // Ensure authentication middleware is enabled
app.UseAuthorization();

app.MapControllers();

app.Run();
