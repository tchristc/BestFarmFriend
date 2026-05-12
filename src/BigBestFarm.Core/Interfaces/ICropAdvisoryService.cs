using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Interfaces;

public interface ICropAdvisoryService
{
    Task<List<CropAdvisory>> GetAdvisoriesAsync(WeatherSnapshot weather, IEnumerable<Crop> crops);
    Task<CropAdvisory> GetAdvisoryAsync(WeatherSnapshot weather, Crop crop, ActionType actionType);
}
