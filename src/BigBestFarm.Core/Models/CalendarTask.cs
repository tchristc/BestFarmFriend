namespace BigBestFarm.Core.Models;

public class CalendarTask
{
    public int Id { get; set; }
    public int CropId { get; set; }
    public GrowthStageCode Stage { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActionType ActionType { get; set; }
    public int SuggestedWeekOfYear { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
