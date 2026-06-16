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

    private static IReadOnlyList<Flight> BuildFlights(DateTime today) =>
    [
        // ── SOF → LHR (London Heathrow) ──────────────────────────────────
        new Flight { Id = "M001", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "LHR",
                     DepartureTime = today.AddDays(1).AddHours(6).AddMinutes(15),
                     ArrivalTime   = today.AddDays(1).AddHours(8).AddMinutes(55),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 49.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M002", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "LHR",
                     DepartureTime = today.AddDays(2).AddHours(10).AddMinutes(30),
                     ArrivalTime   = today.AddDays(2).AddHours(13).AddMinutes(15),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 64.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M003", Airline = "British Airways",  AirlineCode = "BA", Origin = "SOF", Destination = "LHR",
                     DepartureTime = today.AddDays(3).AddHours(7).AddMinutes(0),
                     ArrivalTime   = today.AddDays(3).AddHours(9).AddMinutes(50),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(50)), Price = 189.00m, Currency = "EUR", Stops = 0 },

        new Flight { Id = "M004", Airline = "Lufthansa",        AirlineCode = "LH", Origin = "SOF", Destination = "LHR",
                     DepartureTime = today.AddDays(4).AddHours(14).AddMinutes(20),
                     ArrivalTime   = today.AddDays(4).AddHours(19).AddMinutes(10),
                     Duration = TimeSpan.FromHours(4).Add(TimeSpan.FromMinutes(50)), Price = 215.00m, Currency = "EUR", Stops = 1 },

        new Flight { Id = "M005", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "LHR",
                     DepartureTime = today.AddDays(5).AddHours(18).AddMinutes(45),
                     ArrivalTime   = today.AddDays(5).AddHours(21).AddMinutes(30),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 79.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M006", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "LHR",
                     DepartureTime = today.AddDays(7).AddHours(5).AddMinutes(50),
                     ArrivalTime   = today.AddDays(7).AddHours(8).AddMinutes(35),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 55.49m,  Currency = "EUR", Stops = 0 },

        // ── SOF → TXL (Berlin) ───────────────────────────────────────────
        new Flight { Id = "M007", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "TXL",
                     DepartureTime = today.AddDays(1).AddHours(7).AddMinutes(0),
                     ArrivalTime   = today.AddDays(1).AddHours(9).AddMinutes(15),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(15)), Price = 39.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M008", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "TXL",
                     DepartureTime = today.AddDays(2).AddHours(13).AddMinutes(0),
                     ArrivalTime   = today.AddDays(2).AddHours(15).AddMinutes(20),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(20)), Price = 44.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M009", Airline = "Lufthansa",       AirlineCode = "LH", Origin = "SOF", Destination = "TXL",
                     DepartureTime = today.AddDays(3).AddHours(9).AddMinutes(30),
                     ArrivalTime   = today.AddDays(3).AddHours(11).AddMinutes(50),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(20)), Price = 149.00m, Currency = "EUR", Stops = 0 },

        new Flight { Id = "M010", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "TXL",
                     DepartureTime = today.AddDays(5).AddHours(16).AddMinutes(40),
                     ArrivalTime   = today.AddDays(5).AddHours(19).AddMinutes(0),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(20)), Price = 52.49m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M011", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "TXL",
                     DepartureTime = today.AddDays(6).AddHours(6).AddMinutes(10),
                     ArrivalTime   = today.AddDays(6).AddHours(8).AddMinutes(30),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(20)), Price = 59.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M012", Airline = "Lufthansa",       AirlineCode = "LH", Origin = "SOF", Destination = "TXL",
                     DepartureTime = today.AddDays(8).AddHours(11).AddMinutes(0),
                     ArrivalTime   = today.AddDays(8).AddHours(16).AddMinutes(30),
                     Duration = TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(30)), Price = 179.00m, Currency = "EUR", Stops = 1 },

        // ── SOF → CDG (Paris Charles de Gaulle) ──────────────────────────
        new Flight { Id = "M013", Airline = "Air France",      AirlineCode = "AF", Origin = "SOF", Destination = "CDG",
                     DepartureTime = today.AddDays(1).AddHours(8).AddMinutes(30),
                     ArrivalTime   = today.AddDays(1).AddHours(11).AddMinutes(10),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 129.00m, Currency = "EUR", Stops = 0 },

        new Flight { Id = "M014", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "CDG",
                     DepartureTime = today.AddDays(2).AddHours(15).AddMinutes(20),
                     ArrivalTime   = today.AddDays(2).AddHours(17).AddMinutes(55),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(35)), Price = 69.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M015", Airline = "Air France",      AirlineCode = "AF", Origin = "SOF", Destination = "CDG",
                     DepartureTime = today.AddDays(4).AddHours(6).AddMinutes(0),
                     ArrivalTime   = today.AddDays(4).AddHours(8).AddMinutes(40),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 145.00m, Currency = "EUR", Stops = 0 },

        new Flight { Id = "M016", Airline = "Lufthansa",       AirlineCode = "LH", Origin = "SOF", Destination = "CDG",
                     DepartureTime = today.AddDays(5).AddHours(9).AddMinutes(50),
                     ArrivalTime   = today.AddDays(5).AddHours(15).AddMinutes(20),
                     Duration = TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(30)), Price = 199.00m, Currency = "EUR", Stops = 1 },

        new Flight { Id = "M017", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "CDG",
                     DepartureTime = today.AddDays(7).AddHours(19).AddMinutes(0),
                     ArrivalTime   = today.AddDays(7).AddHours(21).AddMinutes(40),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 54.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M018", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "CDG",
                     DepartureTime = today.AddDays(9).AddHours(12).AddMinutes(30),
                     ArrivalTime   = today.AddDays(9).AddHours(15).AddMinutes(10),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 74.99m,  Currency = "EUR", Stops = 0 },

        // ── SOF → FCO (Rome Fiumicino) ────────────────────────────────────
        new Flight { Id = "M019", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "FCO",
                     DepartureTime = today.AddDays(1).AddHours(11).AddMinutes(0),
                     ArrivalTime   = today.AddDays(1).AddHours(12).AddMinutes(45),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(45)), Price = 42.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M020", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "FCO",
                     DepartureTime = today.AddDays(2).AddHours(7).AddMinutes(40),
                     ArrivalTime   = today.AddDays(2).AddHours(9).AddMinutes(30),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(50)), Price = 48.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M021", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "FCO",
                     DepartureTime = today.AddDays(3).AddHours(16).AddMinutes(0),
                     ArrivalTime   = today.AddDays(3).AddHours(17).AddMinutes(50),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(50)), Price = 59.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M022", Airline = "Lufthansa",       AirlineCode = "LH", Origin = "SOF", Destination = "FCO",
                     DepartureTime = today.AddDays(5).AddHours(8).AddMinutes(30),
                     ArrivalTime   = today.AddDays(5).AddHours(13).AddMinutes(0),
                     Duration = TimeSpan.FromHours(4).Add(TimeSpan.FromMinutes(30)), Price = 175.00m, Currency = "EUR", Stops = 1 },

        new Flight { Id = "M023", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "FCO",
                     DepartureTime = today.AddDays(6).AddHours(20).AddMinutes(15),
                     ArrivalTime   = today.AddDays(6).AddHours(22).AddMinutes(10),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)), Price = 55.49m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M024", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "FCO",
                     DepartureTime = today.AddDays(10).AddHours(6).AddMinutes(0),
                     ArrivalTime   = today.AddDays(10).AddHours(7).AddMinutes(55),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)), Price = 39.99m,  Currency = "EUR", Stops = 0 },

        // ── SOF → AMS (Amsterdam Schiphol) ────────────────────────────────
        new Flight { Id = "M025", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "AMS",
                     DepartureTime = today.AddDays(1).AddHours(9).AddMinutes(0),
                     ArrivalTime   = today.AddDays(1).AddHours(11).AddMinutes(40),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 72.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M026", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "AMS",
                     DepartureTime = today.AddDays(2).AddHours(14).AddMinutes(30),
                     ArrivalTime   = today.AddDays(2).AddHours(17).AddMinutes(15),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 61.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M027", Airline = "Lufthansa",       AirlineCode = "LH", Origin = "SOF", Destination = "AMS",
                     DepartureTime = today.AddDays(3).AddHours(7).AddMinutes(15),
                     ArrivalTime   = today.AddDays(3).AddHours(13).AddMinutes(0),
                     Duration = TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(45)), Price = 220.00m, Currency = "EUR", Stops = 1 },

        new Flight { Id = "M028", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "AMS",
                     DepartureTime = today.AddDays(4).AddHours(18).AddMinutes(0),
                     ArrivalTime   = today.AddDays(4).AddHours(20).AddMinutes(45),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 84.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M029", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "AMS",
                     DepartureTime = today.AddDays(6).AddHours(5).AddMinutes(45),
                     ArrivalTime   = today.AddDays(6).AddHours(8).AddMinutes(30),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 57.49m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M030", Airline = "British Airways", AirlineCode = "BA", Origin = "SOF", Destination = "AMS",
                     DepartureTime = today.AddDays(8).AddHours(11).AddMinutes(0),
                     ArrivalTime   = today.AddDays(8).AddHours(16).AddMinutes(30),
                     Duration = TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(30)), Price = 259.00m, Currency = "EUR", Stops = 1 },

        new Flight { Id = "M031", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "AMS",
                     DepartureTime = today.AddDays(11).AddHours(15).AddMinutes(20),
                     ArrivalTime   = today.AddDays(11).AddHours(18).AddMinutes(5),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 69.99m,  Currency = "EUR", Stops = 0 },

        // ── SOF → MXP (Milan Malpensa) — bonus route ─────────────────────
        new Flight { Id = "M032", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "MXP",
                     DepartureTime = today.AddDays(2).AddHours(7).AddMinutes(0),
                     ArrivalTime   = today.AddDays(2).AddHours(8).AddMinutes(50),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(50)), Price = 36.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M033", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "SOF", Destination = "MXP",
                     DepartureTime = today.AddDays(5).AddHours(17).AddMinutes(30),
                     ArrivalTime   = today.AddDays(5).AddHours(19).AddMinutes(25),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)), Price = 49.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "M034", Airline = "Ryanair",         AirlineCode = "FR", Origin = "SOF", Destination = "MXP",
                     DepartureTime = today.AddDays(9).AddHours(9).AddMinutes(0),
                     ArrivalTime   = today.AddDays(9).AddHours(10).AddMinutes(55),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)), Price = 44.49m,  Currency = "EUR", Stops = 0 },

        // ── LHR → SOF ────────────────────────────────────────────────────
        new Flight { Id = "R001", Airline = "British Airways",  AirlineCode = "BA", Origin = "LHR", Destination = "SOF",
                     DepartureTime = today.AddDays(1).AddHours(11).AddMinutes(0),
                     ArrivalTime   = today.AddDays(1).AddHours(15).AddMinutes(45),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 179.00m, Currency = "EUR", Stops = 0 },

        new Flight { Id = "R002", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "LHR", Destination = "SOF",
                     DepartureTime = today.AddDays(2).AddHours(14).AddMinutes(30),
                     ArrivalTime   = today.AddDays(2).AddHours(19).AddMinutes(10),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 67.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "R003", Airline = "Ryanair",         AirlineCode = "FR", Origin = "LHR", Destination = "SOF",
                     DepartureTime = today.AddDays(4).AddHours(6).AddMinutes(20),
                     ArrivalTime   = today.AddDays(4).AddHours(11).AddMinutes(5),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 52.99m,  Currency = "EUR", Stops = 0 },

        // ── TXL → SOF ────────────────────────────────────────────────────
        new Flight { Id = "R004", Airline = "Lufthansa",       AirlineCode = "LH", Origin = "TXL", Destination = "SOF",
                     DepartureTime = today.AddDays(1).AddHours(10).AddMinutes(0),
                     ArrivalTime   = today.AddDays(1).AddHours(12).AddMinutes(20),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(20)), Price = 139.00m, Currency = "EUR", Stops = 0 },

        new Flight { Id = "R005", Airline = "Ryanair",         AirlineCode = "FR", Origin = "TXL", Destination = "SOF",
                     DepartureTime = today.AddDays(3).AddHours(17).AddMinutes(45),
                     ArrivalTime   = today.AddDays(3).AddHours(20).AddMinutes(5),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(20)), Price = 46.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "R006", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "TXL", Destination = "SOF",
                     DepartureTime = today.AddDays(5).AddHours(8).AddMinutes(30),
                     ArrivalTime   = today.AddDays(5).AddHours(10).AddMinutes(50),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(20)), Price = 58.49m,  Currency = "EUR", Stops = 0 },

        // ── CDG → SOF ────────────────────────────────────────────────────
        new Flight { Id = "R007", Airline = "Air France",      AirlineCode = "AF", Origin = "CDG", Destination = "SOF",
                     DepartureTime = today.AddDays(1).AddHours(13).AddMinutes(0),
                     ArrivalTime   = today.AddDays(1).AddHours(15).AddMinutes(40),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 135.00m, Currency = "EUR", Stops = 0 },

        new Flight { Id = "R008", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "CDG", Destination = "SOF",
                     DepartureTime = today.AddDays(3).AddHours(19).AddMinutes(15),
                     ArrivalTime   = today.AddDays(3).AddHours(21).AddMinutes(55),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 71.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "R009", Airline = "Ryanair",         AirlineCode = "FR", Origin = "CDG", Destination = "SOF",
                     DepartureTime = today.AddDays(6).AddHours(7).AddMinutes(0),
                     ArrivalTime   = today.AddDays(6).AddHours(9).AddMinutes(40),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(40)), Price = 58.99m,  Currency = "EUR", Stops = 0 },

        // ── FCO → SOF ────────────────────────────────────────────────────
        new Flight { Id = "R010", Airline = "Ryanair",         AirlineCode = "FR", Origin = "FCO", Destination = "SOF",
                     DepartureTime = today.AddDays(2).AddHours(13).AddMinutes(30),
                     ArrivalTime   = today.AddDays(2).AddHours(15).AddMinutes(20),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(50)), Price = 44.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "R011", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "FCO", Destination = "SOF",
                     DepartureTime = today.AddDays(4).AddHours(9).AddMinutes(0),
                     ArrivalTime   = today.AddDays(4).AddHours(10).AddMinutes(55),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)), Price = 51.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "R012", Airline = "Lufthansa",       AirlineCode = "LH", Origin = "FCO", Destination = "SOF",
                     DepartureTime = today.AddDays(7).AddHours(16).AddMinutes(0),
                     ArrivalTime   = today.AddDays(7).AddHours(20).AddMinutes(30),
                     Duration = TimeSpan.FromHours(4).Add(TimeSpan.FromMinutes(30)), Price = 168.00m, Currency = "EUR", Stops = 1 },

        // ── AMS → SOF ────────────────────────────────────────────────────
        new Flight { Id = "R013", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "AMS", Destination = "SOF",
                     DepartureTime = today.AddDays(2).AddHours(12).AddMinutes(10),
                     ArrivalTime   = today.AddDays(2).AddHours(14).AddMinutes(55),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 76.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "R014", Airline = "Ryanair",         AirlineCode = "FR", Origin = "AMS", Destination = "SOF",
                     DepartureTime = today.AddDays(4).AddHours(7).AddMinutes(30),
                     ArrivalTime   = today.AddDays(4).AddHours(10).AddMinutes(15),
                     Duration = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(45)), Price = 63.49m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "R015", Airline = "Lufthansa",       AirlineCode = "LH", Origin = "AMS", Destination = "SOF",
                     DepartureTime = today.AddDays(6).AddHours(15).AddMinutes(0),
                     ArrivalTime   = today.AddDays(6).AddHours(20).AddMinutes(45),
                     Duration = TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(45)), Price = 210.00m, Currency = "EUR", Stops = 1 },

        // ── MXP → SOF ────────────────────────────────────────────────────
        new Flight { Id = "R016", Airline = "Ryanair",         AirlineCode = "FR", Origin = "MXP", Destination = "SOF",
                     DepartureTime = today.AddDays(3).AddHours(9).AddMinutes(10),
                     ArrivalTime   = today.AddDays(3).AddHours(11).AddMinutes(5),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)), Price = 38.99m,  Currency = "EUR", Stops = 0 },

        new Flight { Id = "R017", Airline = "Wizz Air",        AirlineCode = "W6", Origin = "MXP", Destination = "SOF",
                     DepartureTime = today.AddDays(6).AddHours(20).AddMinutes(0),
                     ArrivalTime   = today.AddDays(6).AddHours(21).AddMinutes(55),
                     Duration = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)), Price = 47.49m,  Currency = "EUR", Stops = 0 },
    ];
}
