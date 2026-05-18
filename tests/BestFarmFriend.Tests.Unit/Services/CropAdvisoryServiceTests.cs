using BestFarmFriend.Core.Interfaces;
using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Services;

namespace BestFarmFriend.Tests.Unit.Services;

public class CropAdvisoryServiceTests
{
    // -----------------------------------------------------------------------
    // Test doubles
    // -----------------------------------------------------------------------

    private sealed class FakeSprayEngine : ISprayRuleEngine
    {
        public SprayBand Band { get; set; } = SprayBand.Go;
        public double Score { get; set; } = 90;

        public SprayReadinessResult Evaluate(WeatherSnapshot weather) => new()
        {
            Band = Band,
            Score = Score
        };
    }

    private sealed class FakeGrowthStageService : IGrowthStageService
    {
        public GrowthStageCode Stage { get; set; } = GrowthStageCode.FirstCover;

        public GrowthStageCode EstimateCurrentStage(Crop crop, DateOnly date) => Stage;

        public GrowthStageEntry? GetStageEntry(Crop crop, GrowthStageCode stage) =>
            crop.GrowthStages.FirstOrDefault(s => s.Stage == stage);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static CropAdvisoryService CreateService(
        FakeSprayEngine? engine = null,
        FakeGrowthStageService? stageService = null) =>
        new(engine ?? new FakeSprayEngine(), stageService ?? new FakeGrowthStageService());

    private static WeatherSnapshot BaseWeather() => new()
    {
        TemperatureF = 70,
        HumidityPct = 55,
        WindSpeedMph = 4,
        WindGustMph = 8,
        PrecipitationRateInPerHr = 0,
        PrecipPast1hIn = 0,
        PrecipPast24hIn = 0
    };

    private static Crop MakeCrop(params ActionType[] actions) => new()
    {
        Id = 1,
        Name = "Apple",
        Actions = actions.Select(a => new CropAction { ActionType = a, IsEnabled = true }).ToList()
    };

    // -----------------------------------------------------------------------
    // GetAdvisoryAsync — metadata
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAdvisoryAsync_PopulatesCropIdAndName()
    {
        var svc = CreateService();
        var crop = MakeCrop(ActionType.Fertilize);

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), crop, ActionType.Fertilize);

        Assert.Equal(1, advisory.CropId);
        Assert.Equal("Apple", advisory.CropName);
        Assert.Equal(ActionType.Fertilize, advisory.ActionType);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), advisory.Date);
    }

    [Fact]
    public async Task GetAdvisoryAsync_GrowthStage_MatchesFakeService()
    {
        var stageService = new FakeGrowthStageService { Stage = GrowthStageCode.FullBloom };
        var svc = CreateService(stageService: stageService);

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Spray), ActionType.Spray);

        Assert.Equal(GrowthStageCode.FullBloom, advisory.GrowthStage);
    }

    // -----------------------------------------------------------------------
    // GetAdvisoriesAsync — aggregation
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAdvisoriesAsync_ReturnsOneAdvisoryPerEnabledAction()
    {
        var svc = CreateService();
        var crops = new[]
        {
            MakeCrop(ActionType.Spray, ActionType.Fertilize),
            MakeCrop(ActionType.Till)
        };

        var advisories = await svc.GetAdvisoriesAsync(BaseWeather(), crops);

        Assert.Equal(3, advisories.Count);
    }

    [Fact]
    public async Task GetAdvisoriesAsync_SkipsDisabledActions()
    {
        var svc = CreateService();
        var crop = new Crop
        {
            Id = 1,
            Name = "Apple",
            Actions =
            [
                new CropAction { ActionType = ActionType.Spray, IsEnabled = true },
                new CropAction { ActionType = ActionType.Till,  IsEnabled = false }
            ]
        };

        var advisories = await svc.GetAdvisoriesAsync(BaseWeather(), [crop]);

        Assert.Single(advisories);
        Assert.Equal(ActionType.Spray, advisories[0].ActionType);
    }

    [Fact]
    public async Task GetAdvisoriesAsync_EmptyCropList_ReturnsEmpty()
    {
        var svc = CreateService();

        var advisories = await svc.GetAdvisoriesAsync(BaseWeather(), []);

        Assert.Empty(advisories);
    }

    // -----------------------------------------------------------------------
    // EvaluateSpray — all four SprayBand outcomes + FullBloom override
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Spray_GoBand_ReturnsRecommended()
    {
        var engine = new FakeSprayEngine { Band = SprayBand.Go, Score = 90 };
        var svc = CreateService(engine);

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Spray), ActionType.Spray);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
        Assert.Contains("90", advisory.Reasoning);
    }

    [Fact]
    public async Task Spray_CautionBand_ReturnsAcceptable()
    {
        var engine = new FakeSprayEngine { Band = SprayBand.Caution, Score = 70 };
        var svc = CreateService(engine);

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Spray), ActionType.Spray);

        Assert.Equal(AdvisoryStatus.Acceptable, advisory.Status);
    }

    [Fact]
    public async Task Spray_MarginalBand_ReturnsCaution()
    {
        var engine = new FakeSprayEngine { Band = SprayBand.Marginal, Score = 50 };
        var svc = CreateService(engine);

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Spray), ActionType.Spray);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    [Fact]
    public async Task Spray_NoGoBand_ReturnsNotRecommended()
    {
        var engine = new FakeSprayEngine { Band = SprayBand.NoGo, Score = 20 };
        var svc = CreateService(engine);

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Spray), ActionType.Spray);

        Assert.Equal(AdvisoryStatus.NotRecommended, advisory.Status);
    }

    [Fact]
    public async Task Spray_FullBloomStage_ReturnsCautionRegardlessOfBand()
    {
        var engine = new FakeSprayEngine { Band = SprayBand.Go, Score = 95 };
        var stageService = new FakeGrowthStageService { Stage = GrowthStageCode.FullBloom };
        var svc = CreateService(engine, stageService);

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Spray), ActionType.Spray);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
        Assert.Contains("FULL BLOOM", advisory.Reasoning);
    }

    // -----------------------------------------------------------------------
    // EvaluateFertilize
    // Boundaries: wind > 15 → NotRecommended | temp > 90 → Caution
    //             precipPast1h > 0.1 → Caution | else → Recommended
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Fertilize_IdealConditions_ReturnsRecommended()
    {
        var advisory = await CreateService().GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Fertilize), ActionType.Fertilize);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Theory]
    [InlineData(15.1)]
    [InlineData(25)]
    public async Task Fertilize_WindAbove15_ReturnsNotRecommended(double wind)
    {
        var w = BaseWeather(); w.WindSpeedMph = wind;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Fertilize), ActionType.Fertilize);

        Assert.Equal(AdvisoryStatus.NotRecommended, advisory.Status);
    }

    [Fact]
    public async Task Fertilize_WindExactly15_ReturnsRecommended()
    {
        var w = BaseWeather(); w.WindSpeedMph = 15;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Fertilize), ActionType.Fertilize);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Theory]
    [InlineData(90.1)]
    [InlineData(100)]
    public async Task Fertilize_TempAbove90_ReturnsCaution(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Fertilize), ActionType.Fertilize);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    [Fact]
    public async Task Fertilize_PrecipPast1hAbove0_1_ReturnsCaution()
    {
        var w = BaseWeather(); w.PrecipPast1hIn = 0.11;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Fertilize), ActionType.Fertilize);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    [Fact]
    public async Task Fertilize_PrecipExactly0_1_ReturnsRecommended()
    {
        // > 0.1 is false at 0.1
        var w = BaseWeather(); w.PrecipPast1hIn = 0.1;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Fertilize), ActionType.Fertilize);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateTill
    // Boundaries: precipPast24h > 0.5 → NotRecommended | precipPast1h > 0 → Caution
    //             temp < 35 → Caution | else → Recommended
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Till_IdealConditions_ReturnsRecommended()
    {
        var advisory = await CreateService().GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Till), ActionType.Till);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Theory]
    [InlineData(0.51)]
    [InlineData(1.0)]
    public async Task Till_PrecipPast24hAbove0_5_ReturnsNotRecommended(double precip)
    {
        var w = BaseWeather(); w.PrecipPast24hIn = precip;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Till), ActionType.Till);

        Assert.Equal(AdvisoryStatus.NotRecommended, advisory.Status);
    }

    [Fact]
    public async Task Till_PrecipPast24hExactly0_5_ReturnsRecommended()
    {
        // > 0.5 is false at 0.5
        var w = BaseWeather(); w.PrecipPast24hIn = 0.5;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Till), ActionType.Till);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(0.5)]
    public async Task Till_RecentRainPast1h_ReturnsCaution(double precip)
    {
        var w = BaseWeather(); w.PrecipPast1hIn = precip;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Till), ActionType.Till);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    [Theory]
    [InlineData(34.9)]
    [InlineData(20)]
    public async Task Till_TempBelow35_ReturnsCaution(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Till), ActionType.Till);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    [Fact]
    public async Task Till_TempExactly35_ReturnsRecommended()
    {
        // 35 < 35 is false → Recommended
        var w = BaseWeather(); w.TemperatureF = 35;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Till), ActionType.Till);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateGroom
    // Boundaries: FullBloom|PetalFall → Caution | temp < 28 → NotRecommended
    //             precipRate > 0 → Caution | else → Recommended
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Groom_IdealConditions_ReturnsRecommended()
    {
        var advisory = await CreateService().GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Groom), ActionType.Groom);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Theory]
    [InlineData(GrowthStageCode.FullBloom)]
    [InlineData(GrowthStageCode.PetalFall)]
    public async Task Groom_SensitiveStage_ReturnsCaution(GrowthStageCode stage)
    {
        var stageService = new FakeGrowthStageService { Stage = stage };
        var svc = CreateService(stageService: stageService);

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Groom), ActionType.Groom);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
        Assert.Contains("bloom", advisory.Reasoning, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(27.9)]
    [InlineData(0)]
    public async Task Groom_TempBelow28_ReturnsNotRecommended(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Groom), ActionType.Groom);

        Assert.Equal(AdvisoryStatus.NotRecommended, advisory.Status);
    }

    [Fact]
    public async Task Groom_TempExactly28_ReturnsRecommended()
    {
        // 28 < 28 is false → not NotRecommended
        var w = BaseWeather(); w.TemperatureF = 28;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Groom), ActionType.Groom);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Fact]
    public async Task Groom_ActiveRain_ReturnsCaution()
    {
        var w = BaseWeather(); w.PrecipitationRateInPerHr = 0.1;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Groom), ActionType.Groom);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateClean
    // Boundaries: precipRate > 0.1 → Caution | else → Recommended
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Clean_IdealConditions_ReturnsRecommended()
    {
        var advisory = await CreateService().GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Clean), ActionType.Clean);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Theory]
    [InlineData(0.11)]
    [InlineData(0.5)]
    public async Task Clean_HeavyActiveRain_ReturnsCaution(double precipRate)
    {
        var w = BaseWeather(); w.PrecipitationRateInPerHr = precipRate;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Clean), ActionType.Clean);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    [Fact]
    public async Task Clean_PrecipRateExactly0_1_ReturnsRecommended()
    {
        // > 0.1 is false at 0.1
        var w = BaseWeather(); w.PrecipitationRateInPerHr = 0.1;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Clean), ActionType.Clean);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateIrrigate
    // Boundaries: precipPast24h > 0.5 → NotRecommended
    //             temp > 85 && humidity < 40 → Recommended | else → Acceptable
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Irrigate_NeutralConditions_ReturnsAcceptable()
    {
        var advisory = await CreateService().GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Irrigate), ActionType.Irrigate);

        Assert.Equal(AdvisoryStatus.Acceptable, advisory.Status);
    }

    [Theory]
    [InlineData(0.51)]
    [InlineData(2.0)]
    public async Task Irrigate_RecentHeavyRain_ReturnsNotRecommended(double precip24h)
    {
        var w = BaseWeather(); w.PrecipPast24hIn = precip24h;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Irrigate), ActionType.Irrigate);

        Assert.Equal(AdvisoryStatus.NotRecommended, advisory.Status);
        Assert.Contains(precip24h.ToString("F2"), advisory.Reasoning);
    }

    [Fact]
    public async Task Irrigate_PrecipExactly0_5_ReturnsAcceptable()
    {
        // > 0.5 is false at 0.5
        var w = BaseWeather(); w.PrecipPast24hIn = 0.5;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Irrigate), ActionType.Irrigate);

        Assert.Equal(AdvisoryStatus.Acceptable, advisory.Status);
    }

    [Fact]
    public async Task Irrigate_HotAndDry_ReturnsRecommended()
    {
        var w = BaseWeather(); w.TemperatureF = 86; w.HumidityPct = 39;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Irrigate), ActionType.Irrigate);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Fact]
    public async Task Irrigate_HotButHumid_ReturnsAcceptable()
    {
        // temp > 85 but humidity is NOT < 40
        var w = BaseWeather(); w.TemperatureF = 86; w.HumidityPct = 40;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Irrigate), ActionType.Irrigate);

        Assert.Equal(AdvisoryStatus.Acceptable, advisory.Status);
    }

    [Fact]
    public async Task Irrigate_TempExactly85_ReturnsAcceptable()
    {
        // 85 > 85 is false → Acceptable
        var w = BaseWeather(); w.TemperatureF = 85; w.HumidityPct = 20;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Irrigate), ActionType.Irrigate);

        Assert.Equal(AdvisoryStatus.Acceptable, advisory.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateHarvest
    // Boundaries: wind > 20 || gust > 25 → NotRecommended | precipRate > 0 → Caution
    //             temp > 95 → Caution | else → Recommended
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Harvest_IdealConditions_ReturnsRecommended()
    {
        var advisory = await CreateService().GetAdvisoryAsync(BaseWeather(), MakeCrop(ActionType.Harvest), ActionType.Harvest);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Theory]
    [InlineData(20.1, 10)]   // wind just above threshold
    [InlineData(30,   10)]   // wind well above
    [InlineData(5,    25.1)] // gust just above threshold
    [InlineData(5,    35)]   // gust well above
    public async Task Harvest_HighWindOrGust_ReturnsNotRecommended(double wind, double gust)
    {
        var w = BaseWeather(); w.WindSpeedMph = wind; w.WindGustMph = gust;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Harvest), ActionType.Harvest);

        Assert.Equal(AdvisoryStatus.NotRecommended, advisory.Status);
    }

    [Fact]
    public async Task Harvest_WindExactly20_ReturnsRecommended()
    {
        // 20 > 20 is false
        var w = BaseWeather(); w.WindSpeedMph = 20; w.WindGustMph = 10;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Harvest), ActionType.Harvest);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Fact]
    public async Task Harvest_GustExactly25_ReturnsRecommended()
    {
        // 25 > 25 is false
        var w = BaseWeather(); w.WindSpeedMph = 5; w.WindGustMph = 25;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Harvest), ActionType.Harvest);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(0.5)]
    public async Task Harvest_ActiveRain_ReturnsCaution(double precipRate)
    {
        var w = BaseWeather(); w.PrecipitationRateInPerHr = precipRate;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Harvest), ActionType.Harvest);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    [Theory]
    [InlineData(95.1)]
    [InlineData(105)]
    public async Task Harvest_ExtremeHeat_ReturnsCaution(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Harvest), ActionType.Harvest);

        Assert.Equal(AdvisoryStatus.Caution, advisory.Status);
    }

    [Fact]
    public async Task Harvest_TempExactly95_ReturnsRecommended()
    {
        // 95 > 95 is false → Recommended
        var w = BaseWeather(); w.TemperatureF = 95;

        var advisory = await CreateService().GetAdvisoryAsync(w, MakeCrop(ActionType.Harvest), ActionType.Harvest);

        Assert.Equal(AdvisoryStatus.Recommended, advisory.Status);
    }

    // -----------------------------------------------------------------------
    // Unknown ActionType — default case
    // -----------------------------------------------------------------------

    [Fact]
    public async Task UnknownActionType_ReturnsAcceptableWithDefaultReason()
    {
        // Cast an out-of-range int to ActionType to hit the _ switch arm
        var svc = CreateService();
        var actionType = (ActionType)999;

        var advisory = await svc.GetAdvisoryAsync(BaseWeather(), MakeCrop(), actionType);

        Assert.Equal(AdvisoryStatus.Acceptable, advisory.Status);
        Assert.Contains("No specific rules", advisory.Reasoning);
    }
}
