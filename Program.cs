using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Interfaces;
using MovieReservationSystem.Models;
using MovieReservationSystem.Models.CloudStorage;
using MovieReservationSystem.Models.Database;
using MovieReservationSystem.Models.Repositories;
using MovieReservationSystem.Models.UnitOfWork;
using MovieReservationSystem.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Cloud Storage BackBlaze B2 Configuration
var BackBlazeSettings = builder.Configuration.GetSection("BackBlazeB2");

var credentials = new BasicAWSCredentials(
    BackBlazeSettings["KeyId"],
    BackBlazeSettings["ApplicationKey"]
);

var config = new AmazonS3Config
{
    ServiceURL = BackBlazeSettings["ServiceUrl"],
    ForcePathStyle = true
};

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<BearerSecurityOperationTransformer>();
});

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var JwtSettings = builder.Configuration.GetSection("JwtSettings");

//Database and Identity configuration
builder.Services.AddDbContext<appDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("aspmonster")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<appDbContext>()
    .AddDefaultTokenProviders();

//Token Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtSettings["Issuer"],
        ValidAudience = JwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(JwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey is not configured in appsettings.json")))
    };
});

//Injecting Services
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(credentials, config));
builder.Services.AddScoped<IStorageClient, BackBlazeStorage>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

