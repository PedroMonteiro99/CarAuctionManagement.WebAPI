using CarAuctionManagement.Application.Services;
using CarAuctionManagement.Domain.Ports;
using CarAuctionManagement.Infrastructure.Context;
using CarAuctionManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Text;
using CarAuctionManagementAPI.Controllers.Validators;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/carauction-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "CarAuctionManagement")
    .CreateLogger();

try
{
    Log.Information("Starting Car Auction Management API...");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container

    // Configure JWT
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
    var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured.");
    var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured.");
    var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

    builder.Services.AddSingleton(new JwtTokenService(secretKey, issuer, audience, expirationMinutes));

    // Add JWT Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    // Add Entity Framework with InMemory Database
    builder.Services.AddDbContext<AuctionDbContext>(options =>
        options.UseInMemoryDatabase("CarAuctionDb"));

    // Register domain and infrastructure services
    builder.Services.AddScoped<IVehicleRepository, EfVehicleRepository>();
    builder.Services.AddScoped<IAuctionRepository, EfAuctionRepository>();

    // Register validators with dependency injection
    builder.Services.AddScoped<IAddVehicleRequestValidator, AddVehicleRequestValidator>();
    builder.Services.AddScoped<IPlaceBidRequestValidator, PlaceBidRequestValidator>();

    // Register services that depend on validators
    builder.Services.AddScoped<AuctionService>();

    builder.Services.AddControllers();

    // Configure Swagger/Swashbuckle with JWT Bearer support
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "CarAuctionManagement.API",
            Version = "v1",
            Description = "This API provides endpoints for managing vehicle auctions, as well as searching for and adding vehicles."
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Enter 'Bearer' [space] and then your valid JWT token.\r\n\r\nExample: \"Bearer eyJhbG...\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
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

    Log.Information("Car Auction Management API started successfully.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}