using System.Text.Json.Serialization;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MTG_Inventory;
using MTG_Inventory.Mapping;
using MTG_Inventory.Repository;
using MTG_Inventory.Service;
using MTG_Inventory.Service.External.Scryfall;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

var keyVaultName = builder.Configuration["KeyVault:Vault"];
 
if (!string.IsNullOrWhiteSpace(keyVaultName))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}

// Get the connection string from configuration
var connectionString = builder.Configuration["Postgres:ConnectionString"];
logger.LogInformation("PostgreSQL Connection String available: {Available}", connectionString);

// Try alternate key format if not found
if (string.IsNullOrEmpty(connectionString))
{
    logger.LogInformation("Trying alternate connection string format (Postgres--ConnectionString)");
    connectionString = builder.Configuration["Postgres--ConnectionString"];
    logger.LogInformation("Alternate PostgreSQL Connection String available: {Available}", connectionString);
}

if (string.IsNullOrEmpty(connectionString))
{
    logger.LogCritical("No PostgreSQL connection string found in configuration");
    throw new InvalidOperationException("PostgreSQL connection string not found. Please ensure it's configured in the application settings or Key Vault.");
}

// Add health checks with the connection string
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql", tags: new[] { "db", "sql", "postgresql" });


builder.Services.AddCors(options =>

{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetPreflightMaxAge(TimeSpan.FromDays(1));
        });
});

builder.Services.AddHealthChecks();
MappingConfig.Configure();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.AllowOutOfOrderMetadataProperties = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

// Register DbContext with DI container
var configuration = builder.Configuration;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString(connectionString)));

// Register ImportRepository with DI container
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<CardRepository>();

builder.Services.AddScoped<ScryfallService>();

builder.Services.AddScoped<DeckService>();
builder.Services.AddScoped<DeckRepository>();

builder.Services.AddScoped<CardDeckRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();
app.MapHealthChecks("/healthz");
app.MapGet("/", () => "URL Shortener API");
app.MapGet("/health", () => Results.Ok("Healthy"));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
// Configure the HTTP request pipeline

app.UseCors("AllowSpecificOrigins");
// app.UseDelta(shouldExecute: context => context.Request.Path.ToString().Contains("card"));

app.UseHttpsRedirection();
app.MapControllers();

app.Run();