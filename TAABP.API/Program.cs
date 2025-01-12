using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using TAABP.Application;
using TAABP.Application.Profile;
using TAABP.Application.Profile.AmenityMapping;
using TAABP.Application.Profile.CartItemMapping;
using TAABP.Application.Profile.CityMapping;
using TAABP.Application.Profile.CreditCardMapping;
using TAABP.Application.Profile.FeaturedDealMapping;
using TAABP.Application.Profile.HotelMapping;
using TAABP.Application.Profile.PayPalMapping;
using TAABP.Application.Profile.ReservationMapping;
using TAABP.Application.Profile.ReviewMapping;
using TAABP.Application.Profile.RoomMapping;
using TAABP.Application.Profile.UserMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Application.TokenGenerators;
using TAABP.Core;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories;
using TAABP.Infrastructure.Repositories.PaymentRepositories;
using TAABP.Infrastructure.Repositories.ShoppingRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'CarRentalSystemDbContextConnection' not found.");

builder.Services.AddDbContext<TAABPDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TAABPDbContext>();

Log.Logger = new LoggerConfiguration()
     .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserMapper, UserMapper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenGenerator, JWTTokenGenerator>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IHotelMapper, HotelMapper>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IAmenityMapper, AmenityMapper>();
builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomMapper, RoomMapper>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IFeaturedDealService, FeaturedDealService>();
builder.Services.AddScoped<IFeaturedDealMapper, FeaturedDealMapper>();
builder.Services.AddScoped<IFeaturedDealRepository, FeaturedDealRepository>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICityMapper, CityMapper>(); 
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationMapper, ReservationMapper>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewMapper, ReviewMapper>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPaymentOptionService, CreditCardService>();
builder.Services.AddScoped<ICreditCardRepository, CreditCardRepository>();
builder.Services.AddScoped<IPaymentOptionService, PayPalService>();
builder.Services.AddScoped<IPayPalRepository, PayPalRepository>();
builder.Services.AddScoped<PaymentOptionServiceFactory>();
builder.Services.AddScoped<ICreditCardService,  CreditCardService>();
builder.Services.AddScoped<IPayPalService, PayPalService>();
builder.Services.AddScoped<ICreditCardMapper, CreditCardMapper>();
builder.Services.AddScoped<IPayPalMapper, PayPalMapper>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<ICartItemMapper, CartItemMapper>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TAABPDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager<SignInManager<User>>();

var externalAssembly = AppDomain.CurrentDomain.Load("TAABP.Application");
builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(externalAssembly);

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
        ValidAudience = builder.Configuration["Authentication:Audience"],
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]))
    };
});

var app = builder.Build();

async Task SeedAdminUser(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    const string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }
    const string adminEmail = "admin@example.com";
    const string adminPassword = "Admin@1234";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
        else
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}

await SeedAdminUser(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
