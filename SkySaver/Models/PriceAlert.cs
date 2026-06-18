using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SkySaver.Models;

public class PriceAlert : INotifyPropertyChanged
{
    private bool _isActive = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public int Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal? LastCheckedPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastCheckedAt { get; set; }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            _isActive = value;
            OnPropertyChanged();
        }
    }

    public string Currency { get; set; } = "EUR";
    public string DisplayRoute => $"{Origin} → {Destination}";
    public string DisplayDate => DepartureDate.ToString("MMM dd, yyyy");
}
