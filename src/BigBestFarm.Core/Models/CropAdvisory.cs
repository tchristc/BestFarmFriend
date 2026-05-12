namespace BigBestFarm.Core.Models;

public class CropAdvisory
{
    public int Id { get; set; }
    public int CropId { get; set; }
    public string CropName { get; set; } = string.Empty;
    public ActionType ActionType { get; set; }
    public DateOnly Date { get; set; }
    public int LocationId { get; set; }
    public AdvisoryStatus Status { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public GrowthStageCode GrowthStage { get; set; }
    public string? AiNarrative { get; set; }
    public DateTime? AiGeneratedAt { get; set; }
}
