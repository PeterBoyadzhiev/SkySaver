using System.Collections.ObjectModel;
using System.Windows.Input;
using SkySaver.Config;
using SkySaver.Helpers;
using SkySaver.Models;
using SkySaver.Repositories;

namespace SkySaver.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    private readonly IPriceAlertRepository _repo;
    private readonly AppConfig _config;
    private string _statusMessage = string.Empty;
    private readonly HashSet<string> _savedFlightIds = new HashSet<string>();

    public ResultsViewModel(IPriceAlertRepository repo, AppConfig config)
    {
        _repo = repo;
        _config = config;
        SaveAlertCommand = new AsyncRelayCommand<Flight>(SaveAlertAsync);
    }

    public ObservableCollection<Flight> Flights { get; } = new ObservableCollection<Flight>();

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// Returns the data source label set by FlightServiceFactory
    /// (e.g. "Live API (Aviationstack)" or "Mock data (offline)").
    /// </summary>
    public string DataSourceLabel => _config.LastUsedSource;

    public ICommand SaveAlertCommand { get; }

    public void LoadResults(IEnumerable<Flight> flights)
    {
        _savedFlightIds.Clear();
        Flights.Clear();
        foreach (var f in flights)
        {
            f.IsAlertSaved = false;
            Flights.Add(f);
        }
        StatusMessage = string.Empty;
        OnPropertyChanged(nameof(DataSourceLabel));

        // Pre-populate saved state for already-saved alerts (fire-and-forget, best-effort)
        _ = PrePopulateSavedAsync();
    }

    private async Task PrePopulateSavedAsync()
    {
        foreach (var flight in Flights.ToList())
        {
            try
            {
                var exists = await _repo.ExistsAsync(flight.Origin, flight.Destination, flight.DepartureTime.Date);
                if (exists)
                {
                    _savedFlightIds.Add(flight.Id);
                    flight.IsAlertSaved = true;
                }
            }
            catch
            {
                // best effort — ignore individual errors
            }
        }
    }

    public bool IsFlightSaved(string flightId) => _savedFlightIds.Contains(flightId);

    private async Task SaveAlertAsync(Flight? flight)
    {
        if (flight == null) return;
        if (_savedFlightIds.Contains(flight.Id)) return;

        var alert = new PriceAlert
        {
            Origin = flight.Origin,
            Destination = flight.Destination,
            DepartureDate = flight.DepartureTime.Date,
            TargetPrice = flight.Price,
            Currency = flight.Currency,
        };

        await _repo.AddAsync(alert);
        _savedFlightIds.Add(flight.Id);
        flight.IsAlertSaved = true;
        StatusMessage = $"Alert saved! You'll be notified if {flight.Origin}→{flight.Destination} drops below {flight.Price:F2} {flight.Currency}.";
    }
}
