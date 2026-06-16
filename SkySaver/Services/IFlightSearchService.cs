using SkySaver.Models;

namespace SkySaver.Services;

public interface IFlightSearchService
{
    Task<IEnumerable<Flight>> SearchFlightsAsync(SearchQuery query);
    Task<decimal?> GetLowestPriceAsync(string origin, string destination, DateTime date);
}
