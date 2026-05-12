namespace BestFarmFriend.Core.Models;

public enum ActionType
{
    Spray,
    Fertilize,
    Till,
    Groom,
    Clean,
    Irrigate,
    Harvest
}

public enum AdvisoryStatus
{
    Recommended,
    Acceptable,
    Caution,
    NotRecommended
}

public class CropAction
{
    public int Id { get; set; }
    public int CropId { get; set; }
    public ActionType ActionType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;

    // JSON-serialized list of rule overrides (min/max thresholds per weather metric)
    public string? CustomRulesJson { get; set; }
}
