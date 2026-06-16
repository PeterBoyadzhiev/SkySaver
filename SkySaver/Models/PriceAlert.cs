namespace SkySaver.Models;

public class PriceAlert
{
    public int Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal? LastCheckedPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastCheckedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Currency { get; set; } = "EUR";
    public string DisplayRoute => $"{Origin} → {Destination}";
    public string DisplayDate => DepartureDate.ToString("MMM dd, yyyy");
}
