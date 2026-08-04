using NexusERP.Application.Common.Interfaces;
using NexusERP.Application.DependencyInjection;
using NexusERP.Infrastructure.DependencyInjection;
using NexusERP.API.Services;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Azure Key Vault
if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVaultUri"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new Azure.Identity.DefaultAzureCredential());
    }
}

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();

// Add Caching (Using Distributed Memory Cache temporarily instead of Redis for local dev)
builder.Services.AddDistributedMemoryCache();

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Agrega headers: api-supported-versions, api-deprecated-versions
}).AddMvc();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebDashboard",
        policy =>
        {
            policy.WithOrigins("http://localhost:5237", "https://localhost:7136", "https://localhost:7116", "http://localhost:5001", "https://localhost:5001")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
// OpenAPI / Swagger Configuration
builder.Services.AddOpenApi();

// Authentication and Authorization are configured in Infrastructure DependencyInjection
builder.Services.AddAuthorization();

// Configurar Application Insights (Disabled for local dev without connection string)
// builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Inicializar la base de datos y sembrar datos por defecto
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<NexusERP.Infrastructure.Persistence.DatabaseInitializer>();
    await initialiser.InitializeAsync();
    await initialiser.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<NexusERP.API.Middlewares.GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowWebDashboard");

app.UseAuthentication();

app.UseMiddleware<NexusERP.API.Middlewares.TenantMiddleware>();

app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.MapHub<NexusERP.API.Hubs.NotificationHub>("/notificationhub");

app.Run();
