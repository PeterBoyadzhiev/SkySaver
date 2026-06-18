using SkySaver.Models;

namespace SkySaver.Repositories;

public interface IPriceAlertRepository
{
    Task<IEnumerable<PriceAlert>> GetAllAsync();
    Task<IEnumerable<PriceAlert>> GetActiveAsync();
    Task<int> AddAsync(PriceAlert alert);
    Task UpdateAsync(PriceAlert alert);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string origin, string destination, DateTime departureDate);
}
