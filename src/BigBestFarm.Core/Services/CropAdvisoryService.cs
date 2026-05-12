using BigBestFarm.Core.Interfaces;
using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Services;

public class CropAdvisoryService : ICropAdvisoryService
{
    private readonly ISprayRuleEngine _sprayEngine;
    private readonly IGrowthStageService _stageService;

    public CropAdvisoryService(ISprayRuleEngine sprayEngine, IGrowthStageService stageService)
    {
        _sprayEngine = sprayEngine;
        _stageService = stageService;
    }

    public async Task<List<CropAdvisory>> GetAdvisoriesAsync(WeatherSnapshot weather, IEnumerable<Crop> crops)
    {
        var advisories = new List<CropAdvisory>();
        foreach (var crop in crops)
        {
            foreach (var action in crop.Actions.Where(a => a.IsEnabled))
            {
                var advisory = await GetAdvisoryAsync(weather, crop, action.ActionType);
                advisories.Add(advisory);
            }
        }
        return advisories;
    }

    public Task<CropAdvisory> GetAdvisoryAsync(WeatherSnapshot weather, Crop crop, ActionType actionType)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var stage = _stageService.EstimateCurrentStage(crop, today);
        var (status, reasoning) = EvaluateAction(weather, crop, actionType, stage);

        return Task.FromResult(new CropAdvisory
        {
            CropId = crop.Id,
            CropName = crop.Name,
            ActionType = actionType,
            Date = today,
            Status = status,
            Reasoning = reasoning,
            GrowthStage = stage
        });
    }

    private (AdvisoryStatus Status, string Reasoning) EvaluateAction(
        WeatherSnapshot w, Crop crop, ActionType actionType, GrowthStageCode stage)
    {
        return actionType switch
        {
            ActionType.Spray => EvaluateSpray(w, stage),
            ActionType.Fertilize => EvaluateFertilize(w),
            ActionType.Till => EvaluateTill(w),
            ActionType.Groom => EvaluateGroom(w, stage),
            ActionType.Clean => EvaluateClean(w),
            ActionType.Irrigate => EvaluateIrrigate(w),
            ActionType.Harvest => EvaluateHarvest(w),
            _ => (AdvisoryStatus.Acceptable, "No specific rules defined for this action.")
        };
    }

    private (AdvisoryStatus, string) EvaluateSpray(WeatherSnapshot w, GrowthStageCode stage)
    {
        var result = _sprayEngine.Evaluate(w);

        if (stage == GrowthStageCode.FullBloom)
            return (AdvisoryStatus.Caution,
                "FULL BLOOM — Do NOT apply insecticides. Copper only if fire blight is forecast. " +
                $"Spray score: {result.Score}/100 ({result.BandLabel}).");

        return result.Band switch
        {
            SprayBand.Go => (AdvisoryStatus.Recommended, $"Excellent spray conditions. Score: {result.Score}/100."),
            SprayBand.Caution => (AdvisoryStatus.Acceptable, $"Acceptable spray conditions — proceed with awareness. Score: {result.Score}/100."),
            SprayBand.Marginal => (AdvisoryStatus.Caution, $"Marginal conditions — delay if possible. Score: {result.Score}/100."),
            SprayBand.NoGo => (AdvisoryStatus.NotRecommended, $"Do not spray today. Score: {result.Score}/100."),
            _ => (AdvisoryStatus.Caution, "Unable to determine spray readiness.")
        };
    }

    private (AdvisoryStatus, string) EvaluateFertilize(WeatherSnapshot w)
    {
        if (w.WindSpeedMph > 15)
            return (AdvisoryStatus.NotRecommended, "Wind too high for foliar fertilizer application.");
        if (w.TemperatureF > 90)
            return (AdvisoryStatus.Caution, "High temperature — avoid foliar fertilizer; risk of leaf burn.");
        if (w.PrecipPast1hIn > 0.1)
            return (AdvisoryStatus.Caution, "Recent heavy rain — soil fertilizer may leach; foliar will wash off.");
        return (AdvisoryStatus.Recommended, "Good conditions for fertilizing.");
    }

    private (AdvisoryStatus, string) EvaluateTill(WeatherSnapshot w)
    {
        if (w.PrecipPast24hIn > 0.5)
            return (AdvisoryStatus.NotRecommended, "Soil may be saturated — tilling risks compaction and structural damage.");
        if (w.PrecipPast1hIn > 0)
            return (AdvisoryStatus.Caution, "Recent rain — check soil moisture before tilling.");
        if (w.TemperatureF < 35)
            return (AdvisoryStatus.Caution, "Near-freezing temps — ground may be hard; check soil conditions.");
        return (AdvisoryStatus.Recommended, "Good conditions for tilling.");
    }

    private (AdvisoryStatus, string) EvaluateGroom(WeatherSnapshot w, GrowthStageCode stage)
    {
        if (stage == GrowthStageCode.FullBloom || stage == GrowthStageCode.PetalFall)
            return (AdvisoryStatus.Caution, "Avoid heavy pruning during or right after bloom — stress risk.");
        if (w.TemperatureF < 28)
            return (AdvisoryStatus.NotRecommended, "Below 28°F — pruning cuts may freeze and damage the plant.");
        if (w.PrecipitationRateInPerHr > 0)
            return (AdvisoryStatus.Caution, "Raining now — wet cuts can promote fungal disease.");
        return (AdvisoryStatus.Recommended, "Good conditions for grooming/pruning.");
    }

    private (AdvisoryStatus, string) EvaluateClean(WeatherSnapshot w)
    {
        if (w.PrecipitationRateInPerHr > 0.1)
            return (AdvisoryStatus.Caution, "Active rain — equipment cleaning will be less effective; rinse water may spread.");
        return (AdvisoryStatus.Recommended, "Good day for equipment cleaning and orchard cleanup.");
    }

    private (AdvisoryStatus, string) EvaluateIrrigate(WeatherSnapshot w)
    {
        if (w.PrecipPast24hIn > 0.5)
            return (AdvisoryStatus.NotRecommended, $"Significant recent rain ({w.PrecipPast24hIn:F2}\") — irrigation likely not needed.");
        if (w.TemperatureF > 85 && w.HumidityPct < 40)
            return (AdvisoryStatus.Recommended, "Hot and dry — irrigation recommended to reduce heat stress.");
        return (AdvisoryStatus.Acceptable, "Irrigation conditions are acceptable — check soil moisture.");
    }

    private (AdvisoryStatus, string) EvaluateHarvest(WeatherSnapshot w)
    {
        if (w.WindSpeedMph > 20 || w.WindGustMph > 25)
            return (AdvisoryStatus.NotRecommended, "High winds — risk of fruit bruising and worker safety concerns.");
        if (w.PrecipitationRateInPerHr > 0)
            return (AdvisoryStatus.Caution, "Raining — wet harvest increases disease spread and fruit damage.");
        if (w.TemperatureF > 95)
            return (AdvisoryStatus.Caution, "Extreme heat — harvest early morning; fruit may soften quickly.");
        return (AdvisoryStatus.Recommended, "Good conditions for harvesting.");
    }
}
