namespace BigBestFarm.Core.Models;

public enum CropCategory
{
    TreeFruit,
    NutTree,
    Vine,
    Berry,
    Vegetable,
    RowCrop
}

public class Crop
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public CropCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IconSvgKey { get; set; } = string.Empty;
    public string KeyNotes { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<CropAction> Actions { get; set; } = new();
    public List<GrowthStageEntry> GrowthStages { get; set; } = new();
}
