using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Services;

namespace BestFarmFriend.Tests.Unit.Services;

public class GrowthStageServiceTests
{
    private static GrowthStageService CreateService() => new();

    private static Crop CropWithStages() => new()
    {
        Name = "Apple",
        GrowthStages = new List<GrowthStageEntry>
        {
            new() { Stage = GrowthStageCode.Dormant,       TypicalStartDayOfYear = 1,   TypicalEndDayOfYear = 60  },
            new() { Stage = GrowthStageCode.SilverTip,     TypicalStartDayOfYear = 61,  TypicalEndDayOfYear = 75  },
            new() { Stage = GrowthStageCode.GreenTip,      TypicalStartDayOfYear = 76,  TypicalEndDayOfYear = 85  },
            new() { Stage = GrowthStageCode.TightCluster,  TypicalStartDayOfYear = 86,  TypicalEndDayOfYear = 95  },
            new() { Stage = GrowthStageCode.PinkPopcorn,   TypicalStartDayOfYear = 96,  TypicalEndDayOfYear = 110 },
            new() { Stage = GrowthStageCode.FullBloom,     TypicalStartDayOfYear = 111, TypicalEndDayOfYear = 125 },
            new() { Stage = GrowthStageCode.PetalFall,     TypicalStartDayOfYear = 126, TypicalEndDayOfYear = 140 },
            new() { Stage = GrowthStageCode.FruitSet,      TypicalStartDayOfYear = 141, TypicalEndDayOfYear = 155 },
            new() { Stage = GrowthStageCode.FirstCover,    TypicalStartDayOfYear = 156, TypicalEndDayOfYear = 200 },
            new() { Stage = GrowthStageCode.PreHarvest,    TypicalStartDayOfYear = 201, TypicalEndDayOfYear = 270 },
            new() { Stage = GrowthStageCode.PostHarvest,   TypicalStartDayOfYear = 271, TypicalEndDayOfYear = 330 },
            new() { Stage = GrowthStageCode.Dormant,       TypicalStartDayOfYear = 331, TypicalEndDayOfYear = 365 }
        }
    };

    [Fact]
    public void EstimateCurrentStage_Jan1_ReturnsDormant()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 1, 1);

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.Dormant, stage);
    }

    [Fact]
    public void EstimateCurrentStage_MidMarch_ReturnsSilverTip()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 3, 5); // ~DOY 65

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.SilverTip, stage);
    }

    [Fact]
    public void EstimateCurrentStage_LateApril_ReturnsPinkPopcorn()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 4, 15); // ~DOY 106

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.PinkPopcorn, stage);
    }

    [Fact]
    public void EstimateCurrentStage_EarlyMay_ReturnsFullBloom()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 5, 1); // ~DOY 122

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.FullBloom, stage);
    }

    [Fact]
    public void EstimateCurrentStage_Summer_ReturnsFirstCover()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 6, 15); // ~DOY 167

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.FirstCover, stage);
    }

    [Fact]
    public void EstimateCurrentStage_NoCropStages_UsesGenericFallback()
    {
        var service = CreateService();
        var crop = new Crop { Name = "Generic", GrowthStages = new List<GrowthStageEntry>() };

        // DOY 80 => GreenTip in generic fallback
        var date = new DateOnly(2024, 3, 20); // ~DOY 80

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.GreenTip, stage);
    }

    [Fact]
    public void EstimateCurrentStage_GenericFallback_Jan_ReturnsDormant()
    {
        var service = CreateService();
        var crop = new Crop { Name = "Generic", GrowthStages = new List<GrowthStageEntry>() };

        var stage = service.EstimateCurrentStage(crop, new DateOnly(2024, 1, 15));

        Assert.Equal(GrowthStageCode.Dormant, stage);
    }

    [Fact]
    public void EstimateCurrentStage_GenericFallback_Dec_ReturnsDormant()
    {
        var service = CreateService();
        var crop = new Crop { Name = "Generic", GrowthStages = new List<GrowthStageEntry>() };

        var stage = service.EstimateCurrentStage(crop, new DateOnly(2024, 12, 15));

        Assert.Equal(GrowthStageCode.Dormant, stage);
    }

    [Fact]
    public void EstimateCurrentStage_DateOutsideAllStageRanges_ReturnsFallback()
    {
        // Use crop with only one stage in summer; a winter date has no match
        var service = CreateService();
        var crop = new Crop
        {
            Name = "Test",
            GrowthStages = new List<GrowthStageEntry>
            {
                new() { Stage = GrowthStageCode.FullBloom, TypicalStartDayOfYear = 120, TypicalEndDayOfYear = 135 }
            }
        };

        var stage = service.EstimateCurrentStage(crop, new DateOnly(2024, 1, 10));

        // Falls back to closest — only FullBloom exists
        Assert.Equal(GrowthStageCode.FullBloom, stage);
    }

    [Fact]
    public void GetStageEntry_ExistingStage_ReturnsEntry()
    {
        var service = CreateService();
        var crop = CropWithStages();

        var entry = service.GetStageEntry(crop, GrowthStageCode.FullBloom);

        Assert.NotNull(entry);
        Assert.Equal(GrowthStageCode.FullBloom, entry.Stage);
    }

    [Fact]
    public void GetStageEntry_MissingStage_ReturnsNull()
    {
        var service = CreateService();
        var crop = new Crop { Name = "Empty", GrowthStages = new List<GrowthStageEntry>() };

        var entry = service.GetStageEntry(crop, GrowthStageCode.FullBloom);

        Assert.Null(entry);
    }

    // -----------------------------------------------------------------------
    // Crop-based stage boundary tests (TypicalStart/EndDayOfYear inclusive)
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(1,   GrowthStageCode.Dormant)]      // first day of first Dormant window
    [InlineData(60,  GrowthStageCode.Dormant)]      // last day of first Dormant window
    [InlineData(61,  GrowthStageCode.SilverTip)]    // first day of SilverTip
    [InlineData(75,  GrowthStageCode.SilverTip)]    // last day of SilverTip
    [InlineData(76,  GrowthStageCode.GreenTip)]     // first day of GreenTip
    [InlineData(85,  GrowthStageCode.GreenTip)]     // last day of GreenTip
    [InlineData(86,  GrowthStageCode.TightCluster)] // first day of TightCluster
    [InlineData(95,  GrowthStageCode.TightCluster)] // last day of TightCluster
    [InlineData(96,  GrowthStageCode.PinkPopcorn)]  // first day of PinkPopcorn
    [InlineData(110, GrowthStageCode.PinkPopcorn)]  // last day of PinkPopcorn
    [InlineData(111, GrowthStageCode.FullBloom)]    // first day of FullBloom
    [InlineData(125, GrowthStageCode.FullBloom)]    // last day of FullBloom
    [InlineData(126, GrowthStageCode.PetalFall)]    // first day of PetalFall
    [InlineData(140, GrowthStageCode.PetalFall)]    // last day of PetalFall
    [InlineData(141, GrowthStageCode.FruitSet)]     // first day of FruitSet
    [InlineData(155, GrowthStageCode.FruitSet)]     // last day of FruitSet
    [InlineData(156, GrowthStageCode.FirstCover)]   // first day of FirstCover
    [InlineData(200, GrowthStageCode.FirstCover)]   // last day of FirstCover
    [InlineData(201, GrowthStageCode.PreHarvest)]   // first day of PreHarvest
    [InlineData(270, GrowthStageCode.PreHarvest)]   // last day of PreHarvest
    [InlineData(271, GrowthStageCode.PostHarvest)]  // first day of PostHarvest
    [InlineData(330, GrowthStageCode.PostHarvest)]  // last day of PostHarvest
    [InlineData(331, GrowthStageCode.Dormant)]      // first day of second Dormant window
    [InlineData(365, GrowthStageCode.Dormant)]      // last day of second Dormant window
    public void EstimateCurrentStage_CropStages_BoundaryDays(int dayOfYear, GrowthStageCode expected)
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = DateOnly.FromDayNumber(new DateOnly(2024, 1, 1).DayNumber + dayOfYear - 1);

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(expected, stage);
    }

    // -----------------------------------------------------------------------
    // Generic fallback boundary tests (no crop stages defined)
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(1,   GrowthStageCode.Dormant)]          // start of year
    [InlineData(60,  GrowthStageCode.Dormant)]          // last day <= 60
    [InlineData(61,  GrowthStageCode.SilverTip)]        // first day > 60
    [InlineData(75,  GrowthStageCode.SilverTip)]        // last day <= 75
    [InlineData(76,  GrowthStageCode.GreenTip)]         // first day > 75
    [InlineData(85,  GrowthStageCode.GreenTip)]         // last day <= 85
    [InlineData(86,  GrowthStageCode.TightCluster)]     // first day > 85
    [InlineData(95,  GrowthStageCode.TightCluster)]     // last day <= 95
    [InlineData(96,  GrowthStageCode.PinkPopcorn)]      // first day > 95
    [InlineData(110, GrowthStageCode.PinkPopcorn)]      // last day <= 110
    [InlineData(111, GrowthStageCode.FullBloom)]        // first day > 110
    [InlineData(125, GrowthStageCode.FullBloom)]        // last day <= 125
    [InlineData(126, GrowthStageCode.PetalFall)]        // first day > 125
    [InlineData(140, GrowthStageCode.PetalFall)]        // last day <= 140
    [InlineData(141, GrowthStageCode.FruitSet)]         // first day > 140
    [InlineData(155, GrowthStageCode.FruitSet)]         // last day <= 155
    [InlineData(156, GrowthStageCode.FirstCover)]       // first day > 155
    [InlineData(200, GrowthStageCode.FirstCover)]       // last day <= 200
    [InlineData(201, GrowthStageCode.SecondCoverPlus)]  // first day > 200
    [InlineData(240, GrowthStageCode.SecondCoverPlus)]  // last day <= 240
    [InlineData(241, GrowthStageCode.PreHarvest)]       // first day > 240
    [InlineData(270, GrowthStageCode.PreHarvest)]       // last day <= 270
    [InlineData(271, GrowthStageCode.PostHarvest)]      // first day > 270
    [InlineData(330, GrowthStageCode.PostHarvest)]      // last day <= 330
    [InlineData(331, GrowthStageCode.Dormant)]          // first day > 330
    [InlineData(366, GrowthStageCode.Dormant)]          // last day of leap year
    public void EstimateCurrentStage_GenericFallback_BoundaryDays(int dayOfYear, GrowthStageCode expected)
    {
        var service = CreateService();
        var crop = new Crop { Name = "Generic", GrowthStages = new List<GrowthStageEntry>() };
        var date = DateOnly.FromDayNumber(new DateOnly(2024, 1, 1).DayNumber + dayOfYear - 1);

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(expected, stage);
    }

    // -----------------------------------------------------------------------
    // GetStageEntry — duplicate stage returns the first match
    // -----------------------------------------------------------------------

    [Fact]
    public void GetStageEntry_DuplicateStage_ReturnsFirstEntry()
    {
        var service = CreateService();
        var crop = new Crop
        {
            Name = "Apple",
            GrowthStages = new List<GrowthStageEntry>
            {
                new() { Stage = GrowthStageCode.Dormant, TypicalStartDayOfYear = 1,   TypicalEndDayOfYear = 60  },
                new() { Stage = GrowthStageCode.Dormant, TypicalStartDayOfYear = 331, TypicalEndDayOfYear = 365 }
            }
        };

        var entry = service.GetStageEntry(crop, GrowthStageCode.Dormant);

        Assert.NotNull(entry);
        Assert.Equal(1, entry.TypicalStartDayOfYear);
    }

    // -----------------------------------------------------------------------
    // Closest-stage fallback — picks the stage whose start is nearest
    // -----------------------------------------------------------------------

    [Fact]
    public void EstimateCurrentStage_GapBetweenStages_ReturnsClosestByStart()
    {
        var service = CreateService();
        // Gap between DOY 50 and 100; a date of DOY 60 is closer to the first stage's start (10)
        // than to the second stage's start (40 away), so first stage wins.
        var crop = new Crop
        {
            Name = "Test",
            GrowthStages = new List<GrowthStageEntry>
            {
                new() { Stage = GrowthStageCode.Dormant,   TypicalStartDayOfYear = 1,   TypicalEndDayOfYear = 50  },
                new() { Stage = GrowthStageCode.FullBloom, TypicalStartDayOfYear = 100, TypicalEndDayOfYear = 120 }
            }
        };

        // DOY 60: |60-1|=59, |60-100|=40 → closest start is FullBloom (100)
        var stage = service.EstimateCurrentStage(crop, new DateOnly(2024, 2, 29)); // DOY 60

        Assert.Equal(GrowthStageCode.FullBloom, stage);
    }
}
