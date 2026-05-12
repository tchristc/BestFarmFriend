using BestFarmFriend.Core.Models;
using BestFarmFriend.Infrastructure.Data;

namespace BestFarmFriend.Infrastructure.Data.Seed;

public static class CropSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Crops.Any()) return;

        var crops = new List<Crop>
        {
            // ── Tree Fruit ──────────────────────────────────────────────────────────
            new() { Name = "Apple", Slug = "apple", Category = CropCategory.TreeFruit,
                KeyNotes = "High copper/sulfur use; bloom protection critical.",
                IconSvgKey = "apple",
                Actions = DefaultOrchardActions(),
                GrowthStages = AppleStages() },

            new() { Name = "Pear", Slug = "pear", Category = CropCategory.TreeFruit,
                KeyNotes = "Similar to apple; fire blight pressure drives copper timing.",
                IconSvgKey = "pear", Actions = DefaultOrchardActions(), GrowthStages = AppleStages() },

            new() { Name = "Cherry (Sweet)", Slug = "cherry-sweet", Category = CropCategory.TreeFruit,
                KeyNotes = "Avoid copper near harvest; rain-cracking concern.",
                IconSvgKey = "cherry", Actions = DefaultOrchardActions(), GrowthStages = CherryStages() },

            new() { Name = "Cherry (Sour)", Slug = "cherry-sour", Category = CropCategory.TreeFruit,
                KeyNotes = "More tolerant than sweet; spray timing still critical.",
                IconSvgKey = "cherry", Actions = DefaultOrchardActions(), GrowthStages = CherryStages() },

            new() { Name = "Peach", Slug = "peach", Category = CropCategory.TreeFruit,
                KeyNotes = "Brown rot pressure; sulfur timing critical.",
                IconSvgKey = "peach", Actions = DefaultOrchardActions(), GrowthStages = StoneStages() },

            new() { Name = "Nectarine", Slug = "nectarine", Category = CropCategory.TreeFruit,
                KeyNotes = "Similar to peach; less fuzzy skin absorbs more.",
                IconSvgKey = "nectarine", Actions = DefaultOrchardActions(), GrowthStages = StoneStages() },

            new() { Name = "Plum", Slug = "plum", Category = CropCategory.TreeFruit,
                KeyNotes = "Black knot; copper important in dormant season.",
                IconSvgKey = "plum", Actions = DefaultOrchardActions(), GrowthStages = StoneStages() },

            new() { Name = "Apricot", Slug = "apricot", Category = CropCategory.TreeFruit,
                KeyNotes = "Short bloom window; very temperature sensitive.",
                IconSvgKey = "apricot", Actions = DefaultOrchardActions(), GrowthStages = StoneStages() },

            new() { Name = "Fig", Slug = "fig", Category = CropCategory.TreeFruit,
                KeyNotes = "Minimal spray; mainly scale & rust mite with oil.",
                IconSvgKey = "fig", Actions = DefaultOrchardActions(), GrowthStages = new() },

            // ── Nut Trees ────────────────────────────────────────────────────────────
            new() { Name = "Walnut", Slug = "walnut", Category = CropCategory.NutTree,
                KeyNotes = "Codling moth + walnut blight; kaolin clay useful.",
                IconSvgKey = "walnut", Actions = DefaultOrchardActions(), GrowthStages = new() },

            new() { Name = "Almond", Slug = "almond", Category = CropCategory.NutTree,
                KeyNotes = "Bloom spray critical; hull rot late season.",
                IconSvgKey = "almond", Actions = DefaultOrchardActions(), GrowthStages = new() },

            new() { Name = "Hazelnut", Slug = "hazelnut", Category = CropCategory.NutTree,
                KeyNotes = "Eastern Filbert Blight (copper).",
                IconSvgKey = "hazelnut", Actions = DefaultOrchardActions(), GrowthStages = new() },

            new() { Name = "Pecan", Slug = "pecan", Category = CropCategory.NutTree,
                KeyNotes = "Scab pressure; zinc sulfate important.",
                IconSvgKey = "pecan", Actions = DefaultOrchardActions(), GrowthStages = new() },

            // ── Vines ────────────────────────────────────────────────────────────────
            new() { Name = "Grape (Wine)", Slug = "grape-wine", Category = CropCategory.Vine,
                KeyNotes = "Downy/powdery mildew; sulfur and copper on tight schedule.",
                IconSvgKey = "grape", Actions = DefaultOrchardActions(), GrowthStages = GrapeStages() },

            new() { Name = "Grape (Table)", Slug = "grape-table", Category = CropCategory.Vine,
                KeyNotes = "Similar to wine grape; appearance important at harvest.",
                IconSvgKey = "grape", Actions = DefaultOrchardActions(), GrowthStages = GrapeStages() },

            // ── Berries ──────────────────────────────────────────────────────────────
            new() { Name = "Blueberry", Slug = "blueberry", Category = CropCategory.Berry,
                KeyNotes = "Mummy berry; copper at bud swell.",
                IconSvgKey = "blueberry", Actions = DefaultOrchardActions(), GrowthStages = new() },

            new() { Name = "Raspberry", Slug = "raspberry", Category = CropCategory.Berry,
                KeyNotes = "Cane blight; copper in fall.",
                IconSvgKey = "raspberry", Actions = DefaultOrchardActions(), GrowthStages = new() },

            new() { Name = "Blackberry", Slug = "blackberry", Category = CropCategory.Berry,
                KeyNotes = "Cane diseases; copper in fall.",
                IconSvgKey = "blackberry", Actions = DefaultOrchardActions(), GrowthStages = new() },

            new() { Name = "Strawberry", Slug = "strawberry", Category = CropCategory.Berry,
                KeyNotes = "Botrytis pressure; captan timing.",
                IconSvgKey = "strawberry", Actions = DefaultOrchardActions(), GrowthStages = new() },

            // ── Vegetables ───────────────────────────────────────────────────────────
            new() { Name = "Tomato", Slug = "tomato", Category = CropCategory.Vegetable,
                KeyNotes = "Late blight pressure; copper + mancozeb.",
                IconSvgKey = "tomato", Actions = DefaultVegetableActions(), GrowthStages = new() },

            new() { Name = "Potato", Slug = "potato", Category = CropCategory.Vegetable,
                KeyNotes = "Late blight critical; copper/chlorothalonil.",
                IconSvgKey = "potato", Actions = DefaultVegetableActions(), GrowthStages = new() },

            new() { Name = "Pepper", Slug = "pepper", Category = CropCategory.Vegetable,
                KeyNotes = "Phytophthora root rot; avoid overwatering.",
                IconSvgKey = "pepper", Actions = DefaultVegetableActions(), GrowthStages = new() },

            new() { Name = "Corn (Sweet)", Slug = "corn-sweet", Category = CropCategory.Vegetable,
                KeyNotes = "Corn earworm; Northern corn leaf blight.",
                IconSvgKey = "corn", Actions = DefaultVegetableActions(), GrowthStages = new() },

            new() { Name = "Cucumber", Slug = "cucumber", Category = CropCategory.Vegetable,
                KeyNotes = "Downy mildew; angular leaf spot (copper).",
                IconSvgKey = "cucumber", Actions = DefaultVegetableActions(), GrowthStages = new() },

            new() { Name = "Squash / Pumpkin", Slug = "squash", Category = CropCategory.Vegetable,
                KeyNotes = "Powdery mildew; vine borers.",
                IconSvgKey = "squash", Actions = DefaultVegetableActions(), GrowthStages = new() },

            new() { Name = "Beans", Slug = "beans", Category = CropCategory.Vegetable,
                KeyNotes = "White mold; bean beetles.",
                IconSvgKey = "beans", Actions = DefaultVegetableActions(), GrowthStages = new() },

            new() { Name = "Lettuce / Greens", Slug = "lettuce", Category = CropCategory.Vegetable,
                KeyNotes = "Downy mildew; aphids.",
                IconSvgKey = "lettuce", Actions = DefaultVegetableActions(), GrowthStages = new() },

            new() { Name = "Brassicas", Slug = "brassicas", Category = CropCategory.Vegetable,
                KeyNotes = "Clubroot; cabbage loopers; black rot.",
                IconSvgKey = "brassica", Actions = DefaultVegetableActions(), GrowthStages = new() },

            // ── Row Crops ────────────────────────────────────────────────────────────
            new() { Name = "Wheat", Slug = "wheat", Category = CropCategory.RowCrop,
                KeyNotes = "Fusarium head blight; stripe rust.",
                IconSvgKey = "wheat", Actions = DefaultRowCropActions(), GrowthStages = new() },

            new() { Name = "Soybean", Slug = "soybean", Category = CropCategory.RowCrop,
                KeyNotes = "Soybean rust; white mold; aphids.",
                IconSvgKey = "soybean", Actions = DefaultRowCropActions(), GrowthStages = new() },

            new() { Name = "Corn (Field)", Slug = "corn-field", Category = CropCategory.RowCrop,
                KeyNotes = "Gray leaf spot; rootworm; tar spot.",
                IconSvgKey = "corn", Actions = DefaultRowCropActions(), GrowthStages = new() },

            new() { Name = "Alfalfa", Slug = "alfalfa", Category = CropCategory.RowCrop,
                KeyNotes = "Leaf spotting; potato leafhopper; harvest timing critical.",
                IconSvgKey = "alfalfa", Actions = DefaultRowCropActions(), GrowthStages = new() },
        };

        db.Crops.AddRange(crops);
        await db.SaveChangesAsync();
    }

    // ── Shared action templates ──────────────────────────────────────────────────

    private static List<CropAction> DefaultOrchardActions() =>
    [
        new() { ActionType = ActionType.Spray, DisplayName = "Spray", Description = "Apply pesticide or fungicide based on weather conditions and growth stage.", IsEnabled = true },
        new() { ActionType = ActionType.Fertilize, DisplayName = "Fertilize", Description = "Apply fertilizer (soil or foliar) based on soil tests and growth stage.", IsEnabled = true },
        new() { ActionType = ActionType.Groom, DisplayName = "Prune / Groom", Description = "Pruning, thinning, and training based on growth stage and temperature.", IsEnabled = true },
        new() { ActionType = ActionType.Till, DisplayName = "Till / Cultivate", Description = "Under-tree cultivation based on soil moisture and compaction.", IsEnabled = true },
        new() { ActionType = ActionType.Irrigate, DisplayName = "Irrigate", Description = "Irrigation based on ET, soil moisture, and recent precipitation.", IsEnabled = true },
        new() { ActionType = ActionType.Harvest, DisplayName = "Harvest", Description = "Harvest readiness based on temperature, wind, and precipitation.", IsEnabled = true },
        new() { ActionType = ActionType.Clean, DisplayName = "Clean / Sanitize", Description = "Equipment cleaning and orchard sanitation.", IsEnabled = true },
    ];

    private static List<CropAction> DefaultVegetableActions() =>
    [
        new() { ActionType = ActionType.Spray, DisplayName = "Spray", Description = "Apply pesticide or fungicide.", IsEnabled = true },
        new() { ActionType = ActionType.Fertilize, DisplayName = "Fertilize", Description = "Soil or side-dress fertilizer application.", IsEnabled = true },
        new() { ActionType = ActionType.Till, DisplayName = "Till / Cultivate", Description = "Cultivation for weed control and soil aeration.", IsEnabled = true },
        new() { ActionType = ActionType.Irrigate, DisplayName = "Irrigate", Description = "Drip or overhead irrigation.", IsEnabled = true },
        new() { ActionType = ActionType.Harvest, DisplayName = "Harvest", Description = "Harvest readiness assessment.", IsEnabled = true },
    ];

    private static List<CropAction> DefaultRowCropActions() =>
    [
        new() { ActionType = ActionType.Spray, DisplayName = "Spray", Description = "Herbicide or fungicide application.", IsEnabled = true },
        new() { ActionType = ActionType.Fertilize, DisplayName = "Fertilize", Description = "Side-dress or in-furrow fertilizer.", IsEnabled = true },
        new() { ActionType = ActionType.Till, DisplayName = "Till", Description = "Tillage operations.", IsEnabled = true },
        new() { ActionType = ActionType.Harvest, DisplayName = "Harvest", Description = "Harvest readiness and moisture conditions.", IsEnabled = true },
    ];

    // ── Growth stage templates ───────────────────────────────────────────────────

    private static List<GrowthStageEntry> AppleStages() =>
    [
        new() { Stage = GrowthStageCode.Dormant, DisplayName = "Dormant", TypicalStartDayOfYear = 1, TypicalEndDayOfYear = 60, KeyActions = "Dormant oil, copper", Warnings = "Temps must stay above 32°F for 24h after oil" },
        new() { Stage = GrowthStageCode.SilverTip, DisplayName = "Silver Tip", TypicalStartDayOfYear = 61, TypicalEndDayOfYear = 75, KeyActions = "First copper application", Warnings = "Watch for frost" },
        new() { Stage = GrowthStageCode.GreenTip, DisplayName = "Green Tip", TypicalStartDayOfYear = 76, TypicalEndDayOfYear = 85, KeyActions = "Copper + captan", Warnings = "No bloom yet; watch temp" },
        new() { Stage = GrowthStageCode.TightCluster, DisplayName = "Tight Cluster", TypicalStartDayOfYear = 86, TypicalEndDayOfYear = 100, KeyActions = "Copper/captan/sulfur", Warnings = "Key fire blight window" },
        new() { Stage = GrowthStageCode.PinkPopcorn, DisplayName = "Pink / Popcorn", TypicalStartDayOfYear = 101, TypicalEndDayOfYear = 115, KeyActions = "Last pre-bloom spray; insecticide window", Warnings = "Final chance for insecticide before bloom" },
        new() { Stage = GrowthStageCode.FullBloom, DisplayName = "Full Bloom", TypicalStartDayOfYear = 116, TypicalEndDayOfYear = 125, KeyActions = "Copper only if fire blight forecast", Warnings = "NO INSECTICIDES — pollinator protection" },
        new() { Stage = GrowthStageCode.PetalFall, DisplayName = "Petal Fall", TypicalStartDayOfYear = 126, TypicalEndDayOfYear = 140, KeyActions = "Full spray program resumes", Warnings = "Thin fruit; resume cover sprays" },
        new() { Stage = GrowthStageCode.FruitSet, DisplayName = "Fruit Set", TypicalStartDayOfYear = 141, TypicalEndDayOfYear = 155, KeyActions = "Cover sprays begin", Warnings = "Thin to 1 fruit per spur" },
        new() { Stage = GrowthStageCode.FirstCover, DisplayName = "First Cover", TypicalStartDayOfYear = 156, TypicalEndDayOfYear = 175, KeyActions = "Full fungicide/insecticide program", Warnings = "7-14 day intervals" },
        new() { Stage = GrowthStageCode.SecondCoverPlus, DisplayName = "Cover Sprays", TypicalStartDayOfYear = 176, TypicalEndDayOfYear = 240, KeyActions = "Rotate fungicides + insecticide", Warnings = "Check resistance management" },
        new() { Stage = GrowthStageCode.PreHarvest, DisplayName = "Pre-Harvest", TypicalStartDayOfYear = 241, TypicalEndDayOfYear = 270, KeyActions = "Verify PHI for all products", Warnings = "No copper within 30 days of harvest" },
        new() { Stage = GrowthStageCode.PostHarvest, DisplayName = "Post-Harvest", TypicalStartDayOfYear = 271, TypicalEndDayOfYear = 365, KeyActions = "Copper for leaf curl prevention", Warnings = "Apply before leaf drop" },
    ];

    private static List<GrowthStageEntry> CherryStages() =>
    [
        new() { Stage = GrowthStageCode.Dormant, DisplayName = "Dormant", TypicalStartDayOfYear = 1, TypicalEndDayOfYear = 55, KeyActions = "Dormant oil", Warnings = "" },
        new() { Stage = GrowthStageCode.SilverTip, DisplayName = "Silver Tip", TypicalStartDayOfYear = 56, TypicalEndDayOfYear = 70, KeyActions = "Copper spray", Warnings = "Frost risk high" },
        new() { Stage = GrowthStageCode.FullBloom, DisplayName = "Full Bloom", TypicalStartDayOfYear = 100, TypicalEndDayOfYear = 115, KeyActions = "No insecticides", Warnings = "Pollinator protection critical" },
        new() { Stage = GrowthStageCode.PetalFall, DisplayName = "Petal Fall", TypicalStartDayOfYear = 116, TypicalEndDayOfYear = 130, KeyActions = "Spray resume", Warnings = "" },
        new() { Stage = GrowthStageCode.PreHarvest, DisplayName = "Pre-Harvest", TypicalStartDayOfYear = 180, TypicalEndDayOfYear = 220, KeyActions = "No copper; rain-cracking watch", Warnings = "Rain causes fruit splitting" },
        new() { Stage = GrowthStageCode.PostHarvest, DisplayName = "Post-Harvest", TypicalStartDayOfYear = 221, TypicalEndDayOfYear = 365, KeyActions = "Copper for leaf curl", Warnings = "" },
    ];

    private static List<GrowthStageEntry> StoneStages() =>
    [
        new() { Stage = GrowthStageCode.Dormant, DisplayName = "Dormant", TypicalStartDayOfYear = 1, TypicalEndDayOfYear = 60, KeyActions = "Dormant oil, copper", Warnings = "" },
        new() { Stage = GrowthStageCode.PinkPopcorn, DisplayName = "Pre-Bloom", TypicalStartDayOfYear = 80, TypicalEndDayOfYear = 100, KeyActions = "Copper/sulfur", Warnings = "Watch for frost" },
        new() { Stage = GrowthStageCode.FullBloom, DisplayName = "Full Bloom", TypicalStartDayOfYear = 101, TypicalEndDayOfYear = 115, KeyActions = "No insecticides", Warnings = "Pollinator protection" },
        new() { Stage = GrowthStageCode.FruitSet, DisplayName = "Fruit Set", TypicalStartDayOfYear = 116, TypicalEndDayOfYear = 150, KeyActions = "Sulfur for brown rot", Warnings = "High humidity = high risk" },
        new() { Stage = GrowthStageCode.PreHarvest, DisplayName = "Pre-Harvest", TypicalStartDayOfYear = 200, TypicalEndDayOfYear = 250, KeyActions = "Verify PHI", Warnings = "Brown rot pressure near harvest" },
        new() { Stage = GrowthStageCode.PostHarvest, DisplayName = "Post-Harvest", TypicalStartDayOfYear = 251, TypicalEndDayOfYear = 365, KeyActions = "Copper", Warnings = "" },
    ];

    private static List<GrowthStageEntry> GrapeStages() =>
    [
        new() { Stage = GrowthStageCode.Dormant, DisplayName = "Dormant", TypicalStartDayOfYear = 1, TypicalEndDayOfYear = 90, KeyActions = "Dormant oil if needed", Warnings = "" },
        new() { Stage = GrowthStageCode.GreenTip, DisplayName = "Bud Swell", TypicalStartDayOfYear = 91, TypicalEndDayOfYear = 110, KeyActions = "Copper for downy mildew", Warnings = "Frost risk" },
        new() { Stage = GrowthStageCode.TightCluster, DisplayName = "5-Leaf Stage", TypicalStartDayOfYear = 111, TypicalEndDayOfYear = 140, KeyActions = "Sulfur/copper on 7-10 day schedule", Warnings = "Downy + powdery mildew risk" },
        new() { Stage = GrowthStageCode.FullBloom, DisplayName = "Bloom", TypicalStartDayOfYear = 141, TypicalEndDayOfYear = 160, KeyActions = "Sulfur carefully; no oils", Warnings = "Critical fungicide window" },
        new() { Stage = GrowthStageCode.FruitSet, DisplayName = "Fruit Set", TypicalStartDayOfYear = 161, TypicalEndDayOfYear = 200, KeyActions = "Continue spray schedule", Warnings = "Botrytis risk at cluster closure" },
        new() { Stage = GrowthStageCode.PreHarvest, DisplayName = "Veraison / Pre-Harvest", TypicalStartDayOfYear = 201, TypicalEndDayOfYear = 270, KeyActions = "Verify PHI; Botrytis watch", Warnings = "Stop sulfur 4 weeks before harvest" },
        new() { Stage = GrowthStageCode.PostHarvest, DisplayName = "Post-Harvest", TypicalStartDayOfYear = 271, TypicalEndDayOfYear = 365, KeyActions = "Copper after leaf drop", Warnings = "" },
    ];
}
