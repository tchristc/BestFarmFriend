using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Interfaces;

public interface ICropRepository
{
    Task<List<Crop>> GetAllAsync();
    Task<Crop?> GetByIdAsync(int id);
    Task<Crop?> GetBySlugAsync(string slug);
    Task<List<Crop>> GetActiveCropsAsync();
}
