using SkySaver.Models;

namespace SkySaver.Services;

public interface IFlightService
{
    Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime date);
    Task<decimal?> GetLowestPriceAsync(string origin, string destination, DateTime date);
}
