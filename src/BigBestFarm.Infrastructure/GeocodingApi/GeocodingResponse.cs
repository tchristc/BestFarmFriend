using System.Text.Json.Serialization;

namespace BigBestFarm.Infrastructure.GeocodingApi;

public class GeocodingResponse
{
    [JsonPropertyName("results")]
    public List<GeocodingResult> Results { get; set; } = new();
}

public class GeocodingResult
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("admin1")]
    public string? Admin1 { get; set; }

    [JsonPropertyName("postcodes")]
    public List<string>? Postcodes { get; set; }
}
