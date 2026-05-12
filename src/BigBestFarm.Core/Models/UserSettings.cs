namespace BigBestFarm.Core.Models;

public class UserSettings
{
    public int PreferredLocationId { get; set; }
    public bool UseMetricUnits { get; set; } = false;
    public bool DarkMode { get; set; } = false;
    public string AiProvider { get; set; } = "OpenAI";
    public string? AiApiKey { get; set; }
    public string AiModel { get; set; } = "gpt-4o";
    public List<int> ActiveCropIds { get; set; } = new();
}
