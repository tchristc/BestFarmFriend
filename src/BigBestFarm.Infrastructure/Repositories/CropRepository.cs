using BigBestFarm.Core.Interfaces;
using BigBestFarm.Core.Models;
using BigBestFarm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BigBestFarm.Infrastructure.Repositories;

public class CropRepository : ICropRepository
{
    private readonly AppDbContext _db;

    public CropRepository(AppDbContext db) => _db = db;

    public Task<List<Crop>> GetAllAsync() =>
        _db.Crops
            .Include(c => c.Actions)
            .Include(c => c.GrowthStages)
            .OrderBy(c => c.Category).ThenBy(c => c.Name)
            .ToListAsync();

    public Task<Crop?> GetByIdAsync(int id) =>
        _db.Crops
            .Include(c => c.Actions)
            .Include(c => c.GrowthStages)
            .FirstOrDefaultAsync(c => c.Id == id);

    public Task<Crop?> GetBySlugAsync(string slug) =>
        _db.Crops
            .Include(c => c.Actions)
            .Include(c => c.GrowthStages)
            .FirstOrDefaultAsync(c => c.Slug == slug);

    public Task<List<Crop>> GetActiveCropsAsync() =>
        _db.Crops
            .Include(c => c.Actions)
            .Include(c => c.GrowthStages)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Category).ThenBy(c => c.Name)
            .ToListAsync();
}
