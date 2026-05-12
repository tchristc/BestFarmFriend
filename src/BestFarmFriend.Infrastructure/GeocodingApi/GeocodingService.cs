using System.Net.Http.Json;
using BestFarmFriend.Core.Interfaces;
using BestFarmFriend.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BestFarmFriend.Infrastructure.GeocodingApi;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GeocodingService> _logger;

    private const string SearchUrl = "https://geocoding-api.open-meteo.com/v1/search";

    public GeocodingService(HttpClient http, IMemoryCache cache, ILogger<GeocodingService> logger)
    {
        _http = http;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<Location>> SearchAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return new List<Location>();

        string cacheKey = $"geo_{query.ToLowerInvariant().Trim()}";
        if (_cache.TryGetValue(cacheKey, out List<Location>? cached) && cached != null)
            return cached;

        try
        {
            string url = $"{SearchUrl}?name={Uri.EscapeDataString(query)}&count=8&language=en&format=json";
            var response = await _http.GetFromJsonAsync<GeocodingResponse>(url, ct);
            var results = response?.Results.Select(r => new Location
            {
                DisplayName = BuildDisplayName(r),
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                Timezone = r.Timezone ?? "UTC",
                City = r.Name,
                State = r.Admin1,
                Country = r.Country,
                ZipCode = r.Postcodes?.FirstOrDefault()
            }).ToList() ?? new List<Location>();

            _cache.Set(cacheKey, results, TimeSpan.FromHours(24));
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Geocoding search failed for query: {Query}", query);
            return new List<Location>();
        }
    }

    public Task<Location?> ResolveByCoordinatesAsync(double latitude, double longitude, CancellationToken ct = default)
    {
        // Open-Meteo geocoding doesn't support reverse geocoding; return a best-effort location
        return Task.FromResult<Location?>(new Location
        {
            DisplayName = $"{latitude:F4}, {longitude:F4}",
            Latitude = latitude,
            Longitude = longitude,
            Timezone = "UTC"
        });
    }

    private static string BuildDisplayName(GeocodingResult r)
    {
        var parts = new List<string> { r.Name };
        if (!string.IsNullOrEmpty(r.Admin1)) parts.Add(r.Admin1);
        if (!string.IsNullOrEmpty(r.CountryCode)) parts.Add(r.CountryCode);
        return string.Join(", ", parts);
    }
}
