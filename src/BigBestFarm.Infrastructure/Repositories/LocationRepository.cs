using BigBestFarm.Core.Interfaces;
using BigBestFarm.Core.Models;
using BigBestFarm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BigBestFarm.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _db;

    public LocationRepository(AppDbContext db) => _db = db;

    public Task<List<Location>> GetAllAsync() =>
        _db.Locations.OrderByDescending(l => l.LastUsed).ToListAsync();

    public Task<Location?> GetByIdAsync(int id) =>
        _db.Locations.FindAsync(id).AsTask();

    public async Task<Location> AddAsync(Location location)
    {
        _db.Locations.Add(location);
        await _db.SaveChangesAsync();
        return location;
    }

    public async Task UpdateAsync(Location location)
    {
        _db.Locations.Update(location);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var loc = await _db.Locations.FindAsync(id);
        if (loc != null)
        {
            _db.Locations.Remove(loc);
            await _db.SaveChangesAsync();
        }
    }
}
