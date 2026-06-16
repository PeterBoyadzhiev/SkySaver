using SkySaver.Models;

namespace SkySaver.Services;

/// <summary>
/// Tries the primary service; falls back to secondary if primary throws or returns no results.
/// </summary>
public class FallbackFlightService : IFlightService
{
    private readonly IFlightService _primary;
    private readonly IFlightService _fallback;

    public FallbackFlightService(IFlightService primary, IFlightService fallback)
    {
        _primary  = primary;
        _fallback = fallback;
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime date)
    {
        try
        {
            var results = (await _primary.SearchFlightsAsync(origin, destination, date)).ToList();
            if (results.Count > 0) return results;
        }
        catch { }

        return await _fallback.SearchFlightsAsync(origin, destination, date);
    }

    public async Task<decimal?> GetLowestPriceAsync(string origin, string destination, DateTime date)
    {
        try
        {
            var price = await _primary.GetLowestPriceAsync(origin, destination, date);
            if (price.HasValue) return price;
        }
        catch { }

        return await _fallback.GetLowestPriceAsync(origin, destination, date);
    }
}
