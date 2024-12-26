using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TAABP.Application.Profile;
using TAABP.Application.Profile.AmenityMapping;
using TAABP.Application.Profile.CityMapping;
using TAABP.Application.Profile.FeaturedDealMapping;
using TAABP.Application.Profile.HotelMapping;
using TAABP.Application.Profile.ReservationMapping;
using TAABP.Application.Profile.RoomMapping;
using TAABP.Application.Profile.UserMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Application.TokenGenerators;
using TAABP.Core;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'CarRentalSystemDbContextConnection' not found.");

builder.Services.AddDbContext<TAABPDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TAABPDbContext>();

builder.Services.AddControllers()
    .AddNewtonsoftJson();
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
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TAABPDbContext>()
    .AddSignInManager<SignInManager<User>>();

var externalAssembly = AppDomain.CurrentDomain.Load("TAABP.Application");
builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(externalAssembly);
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

// Configure the HTTP request pipeline.
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
