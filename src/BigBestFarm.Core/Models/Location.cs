namespace BigBestFarm.Core.Models;

public class Location
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
}
