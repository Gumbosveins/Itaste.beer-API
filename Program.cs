using ItbApi;
using ItbApi.DataAccessLayer;
using ItbApi.Helpers;
using ItbApi.Settings;
using ItbApi.TaterBusiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReviewerAPI.BeerHubs;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Itaste.beer API",
        Version = "v1",
        Description = "Public API for Itaste.beer."
    });

    // Use full type names to avoid schema ID collisions
    options.CustomSchemaIds(type => type.FullName);

    // Include XML comments (enable in csproj) for richer Swagger docs
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:8090", "https://itaste.beer", "https://localhost:5001") // Replace with your actual frontend URL(s)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Necessary for SignalR WebSockets with credentials
    });
});

builder.Services.AddHttpContextAccessor();

// Load configuration and settings
var configuration = builder.Configuration;
var untappedSettings = configuration.GetSection("Untapped").Get<UntappedSettings>();

if (untappedSettings is null)
{
    throw new Exception("Untapped Settings missing in configuration");
}

builder.Services.AddDbContext<ItbContext>(options =>
    options.UseSqlServer(configuration.GetValue<string>("DBURL")));

// Register settings and services with DI
builder.Services.AddSingleton(untappedSettings);
builder.Services.TryAddScoped<IUnTapped, UnTapped>();
builder.Services.TryAddScoped<ITasterBusiness, TasterBusiness>();
builder.Services.TryAddScoped<ITasterDal, TasterDal>();
builder.Services.TryAddScoped<INotificationService, NotificationService>();
builder.Services.AddSignalR();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Enable Swagger and Swagger UI in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Itaste.beer API v1");
    c.RoutePrefix = "swagger"; // UI at /swagger
});

// Configure Middleware
app.UseCors("AllowAll"); // Ensure CORS is configured before routing
app.UseHttpsRedirection();
app.UseRouting();

// Endpoint Configuration
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<BeerHub>("/beerhub"); // Map the SignalR hub
});

app.Run();
