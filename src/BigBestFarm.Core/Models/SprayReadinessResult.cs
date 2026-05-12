namespace BigBestFarm.Core.Models;

public enum SprayFactor
{
    Wind,
    Precipitation,
    Temperature,
    Humidity,
    TimeOfDay
}

public enum FactorStatus
{
    Pass,
    Marginal,
    Fail
}

public enum SprayBand
{
    Go,
    Caution,
    Marginal,
    NoGo
}

public enum ProductType
{
    Copper,
    Sulfur,
    Oil,
    Insecticide,
    Herbicide
}

public class SprayFactorResult
{
    public SprayFactor Factor { get; set; }
    public string CurrentValue { get; set; } = string.Empty;
    public FactorStatus Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class ProductReadinessResult
{
    public ProductType ProductType { get; set; }
    public bool IsRecommended { get; set; }
    public FactorStatus Status { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class SprayReadinessResult
{
    public double Score { get; set; }
    public SprayBand Band { get; set; }
    public List<SprayFactorResult> FactorResults { get; set; } = new();
    public List<ProductReadinessResult> ProductResults { get; set; } = new();
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    public string BandLabel => Band switch
    {
        SprayBand.Go => "Go",
        SprayBand.Caution => "Caution",
        SprayBand.Marginal => "Marginal",
        SprayBand.NoGo => "No-Go",
        _ => "Unknown"
    };

    public string BandCssClass => Band switch
    {
        SprayBand.Go => "band-go",
        SprayBand.Caution => "band-caution",
        SprayBand.Marginal => "band-marginal",
        SprayBand.NoGo => "band-nogo",
        _ => ""
    };
}
