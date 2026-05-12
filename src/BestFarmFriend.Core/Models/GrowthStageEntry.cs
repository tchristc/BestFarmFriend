namespace BestFarmFriend.Core.Models;

public enum GrowthStageCode
{
    Dormant,
    SilverTip,
    GreenTip,
    HalfInchGreen,
    TightCluster,
    PinkPopcorn,
    FullBloom,
    PetalFall,
    FruitSet,
    FirstCover,
    SecondCoverPlus,
    PreHarvest,
    PostHarvest
}

public class GrowthStageEntry
{
    public int Id { get; set; }
    public int CropId { get; set; }
    public GrowthStageCode Stage { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int TypicalStartDayOfYear { get; set; }
    public int TypicalEndDayOfYear { get; set; }
    public string KeyActions { get; set; } = string.Empty;
    public string Warnings { get; set; } = string.Empty;
}
