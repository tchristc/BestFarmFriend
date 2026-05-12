using BigBestFarm.Core.Interfaces;
using BigBestFarm.Core.Services;
using BigBestFarm.Infrastructure.AiApi;
using BigBestFarm.Infrastructure.Data;
using BigBestFarm.Infrastructure.GeocodingApi;
using BigBestFarm.Infrastructure.Repositories;
using BigBestFarm.Infrastructure.WeatherApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BigBestFarm.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBigBestFarmInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // HTTP clients
        services.AddHttpClient<IWeatherService, WeatherService>(c =>
            c.BaseAddress = new Uri("https://api.open-meteo.com"));

        services.AddHttpClient<IGeocodingService, GeocodingService>(c =>
            c.BaseAddress = new Uri("https://geocoding-api.open-meteo.com"));

        // Repositories
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ICropRepository, CropRepository>();

        // Core services
        services.AddScoped<ISprayRuleEngine, SprayRuleEngine>();
        services.AddScoped<IGrowthStageService, GrowthStageService>();
        services.AddScoped<ICropAdvisoryService, CropAdvisoryService>();

        // AI
        services.AddScoped<IAiAdvisoryService, AiAdvisoryService>();

        // Caching
        services.AddMemoryCache();

        return services;
    }
}
