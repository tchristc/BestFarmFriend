using BestFarmFriend.Core.Models;

namespace BestFarmFriend.Core.Interfaces;

public interface IGeocodingService
{
    Task<List<Location>> SearchAsync(string query, CancellationToken ct = default);
    Task<Location?> ResolveByCoordinatesAsync(double latitude, double longitude, CancellationToken ct = default);
}
