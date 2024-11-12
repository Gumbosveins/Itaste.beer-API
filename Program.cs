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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:8090") // Replace with your actual frontend URL(s)
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

// Enable Swagger and Swagger UI only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
