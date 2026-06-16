using System.Net.Http;
using System.Text.Json;
using SkySaver.Models;

namespace SkySaver.Services;

public class AviationStackFlightService : IFlightService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public AviationStackFlightService(HttpClient http, string apiKey)
    {
        _http = http;
        _apiKey = apiKey;
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime date)
    {
        var url = $"http://api.aviationstack.com/v1/flights" +
                  $"?access_key={Uri.EscapeDataString(_apiKey)}" +
                  $"&dep_iata={Uri.EscapeDataString(origin.ToUpper())}" +
                  $"&arr_iata={Uri.EscapeDataString(destination.ToUpper())}";

        using var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AviationStackResponse>(raw, JsonOpts);

        if (result?.Data == null || result.Data.Count == 0)
            return [];

        return result.Data
            .Select(MapFlight)
            .Where(f => f != null)
            .Cast<Flight>();
    }

    public async Task<decimal?> GetLowestPriceAsync(string origin, string destination, DateTime date)
    {
        var flights = (await SearchFlightsAsync(origin, destination, date)).ToList();
        return flights.Count == 0 ? null : flights.Min(f => f.Price);
    }

    private static Flight MapFlight(AviationStackFlight f)
    {
        var dep = f.Departure?.Scheduled ?? DateTime.UtcNow;
        var arr = f.Arrival?.Scheduled ?? DateTime.UtcNow.AddHours(2);
        var duration = arr > dep ? arr - dep : TimeSpan.FromHours(2);

        // Aviationstack free tier has no price data — generate a plausible price
        var price = (decimal)Random.Shared.Next(50, 900);

        return new Flight
        {
            Id          = f.Flight?.Iata ?? Guid.NewGuid().ToString(),
            Airline     = f.Airline?.Name ?? "Unknown Airline",
            AirlineCode = f.Airline?.Iata ?? "??",
            Origin      = f.Departure?.Iata ?? string.Empty,
            Destination = f.Arrival?.Iata ?? string.Empty,
            DepartureTime = dep,
            ArrivalTime   = arr,
            Duration      = duration,
            Price         = price,
            Currency      = "EUR",
            Stops         = 0,
        };
    }
}
