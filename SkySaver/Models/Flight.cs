namespace SkySaver.Models;

public class Flight
{
    public string Id { get; set; } = string.Empty;
    public string Airline { get; set; } = string.Empty;
    public string AirlineCode { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public int Stops { get; set; }
    public string DurationDisplay => Duration.TotalHours >= 1
        ? $"{(int)Duration.TotalHours}h {Duration.Minutes}m"
        : $"{Duration.Minutes}m";
    public string StopsDisplay => Stops == 0 ? "Nonstop" : $"{Stops} stop{(Stops > 1 ? "s" : "")}";
}
