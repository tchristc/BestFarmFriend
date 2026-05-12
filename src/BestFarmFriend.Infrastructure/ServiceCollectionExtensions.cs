using BestFarmFriend.Core.Interfaces;
using BestFarmFriend.Core.Services;
using BestFarmFriend.Infrastructure.AiApi;
using BestFarmFriend.Infrastructure.Data;
using BestFarmFriend.Infrastructure.GeocodingApi;
using BestFarmFriend.Infrastructure.Repositories;
using BestFarmFriend.Infrastructure.WeatherApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BestFarmFriend.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBestFarmFriendInfrastructure(
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
