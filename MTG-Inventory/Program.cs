using System.Text.Json.Serialization;
using Delta;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using MTG_Inventory;
using MTG_Inventory.Mapping;
using MTG_Inventory.Repository;
using MTG_Inventory.Service;
using MTG_Inventory.Service.External.Scryfall;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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