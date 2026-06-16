using SkySaver.Models;

namespace SkySaver.Services;

public class MockFlightService : IFlightService
{
    private readonly IReadOnlyList<Flight> _flights;

    public MockFlightService()
    {
        _flights = BuildFlights(DateTime.Today);
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime date)
    {
        await Task.Delay(800);
        return _flights
            .Where(f => f.Origin.Equals(origin.Trim(), StringComparison.OrdinalIgnoreCase)
                     && f.Destination.Equals(destination.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<decimal?> GetLowestPriceAsync(string origin, string destination, DateTime date)
    {
        var results = (await SearchFlightsAsync(origin, destination, date)).ToList();
        return results.Count == 0 ? null : results.Min(f => f.Price);
    }

    private static IReadOnlyList<Flight> BuildFlights(DateTime today)
    {
        var airports = new[] { "SOF", "LHR", "TXL", "CDG", "FCO", "AMS", "MXP" };
        var carriers = new (string Name, string Code)[]
        {
            ("Ryanair", "FR"), ("Wizz Air", "W6"), ("Lufthansa", "LH"), ("British Airways", "BA"), ("Air France", "AF")
        };

        var flights = new List<Flight>();
        int idCounter = 1;

        for (int i = 0; i < airports.Length; i++)
        {
            for (int j = 0; j < airports.Length; j++)
            {
                if (i == j) continue;
                var origin = airports[i];
                var dest = airports[j];

                // create 2 sample flights per origin-destination pair
                for (int k = 0; k < 2; k++)
                {
                    var carrier = carriers[(idCounter + k) % carriers.Length];
                    var dep = today.AddDays(1 + ((idCounter + k) % 10)).AddHours(6 + ((idCounter + k) % 12));
                    var durationHours = 1 + Math.Abs(i - j); // simple deterministic duration
                    var duration = TimeSpan.FromHours(durationHours).Add(TimeSpan.FromMinutes((idCounter * 7) % 60));
                    var arr = dep.Add(duration);
                    var basePrice = 30 + (Math.Abs(i - j) * 20) + (k * 10);
                    var price = Math.Round((decimal)basePrice + (idCounter % 7) * 3, 2);

                    flights.Add(new Flight
                    {
                        Id = $"G{idCounter:000}",
                        Airline = carrier.Name,
                        AirlineCode = carrier.Code,
                        Origin = origin,
                        Destination = dest,
                        DepartureTime = dep,
                        ArrivalTime = arr,
                        Duration = duration,
                        Price = price,
                        Currency = "EUR",
                        Stops = (Math.Abs(i - j) > 2) ? 1 : 0
                    });

                    idCounter++;
                }
            }
        }

        return flights;
    }
}
