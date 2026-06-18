namespace SkySaver.Models;

public class SearchQuery
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; } = DateTime.Today.AddDays(7);
    public int Adults { get; set; } = 1;
    public DateTime? ReturnDate { get; set; }
    public bool IsRoundTrip => ReturnDate.HasValue;
}
