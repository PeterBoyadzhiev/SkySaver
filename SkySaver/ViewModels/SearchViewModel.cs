using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows.Input;
using SkySaver.Helpers;
using SkySaver.Models;
using SkySaver.Services;

namespace SkySaver.ViewModels;

public class SearchViewModel : ViewModelBase
{
    private readonly IFlightServiceFactory _factory;

    private string _origin = string.Empty;
    private string _destination = string.Empty;
    private DateTime _departureDate = DateTime.Today.AddDays(7);
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    public SearchViewModel(IFlightServiceFactory factory)
    {
        _factory = factory;
        SearchCommand = new AsyncRelayCommand(SearchAsync, () => CanSearch);
    }

    public string Origin
    {
        get => _origin;
        set { if (SetProperty(ref _origin, value.ToUpper())) OnPropertyChanged(nameof(CanSearch)); }
    }

    public string Destination
    {
        get => _destination;
        set { if (SetProperty(ref _destination, value.ToUpper())) OnPropertyChanged(nameof(CanSearch)); }
    }

    public DateTime DepartureDate
    {
        get => _departureDate;
        set { if (SetProperty(ref _departureDate, value)) OnPropertyChanged(nameof(CanSearch)); }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set { if (SetProperty(ref _isLoading, value)) OnPropertyChanged(nameof(CanSearch)); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { SetProperty(ref _errorMessage, value); OnPropertyChanged(nameof(HasError)); }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public bool CanSearch =>
        !IsLoading &&
        Origin.Length == 3 &&
        Destination.Length == 3 &&
        DepartureDate >= DateTime.Today;

    public ObservableCollection<Flight> Results { get; } = [];

    public ICommand SearchCommand { get; }

    public event Action<IEnumerable<Flight>>? SearchCompleted;

    private async Task SearchAsync()
    {
        ErrorMessage = string.Empty;
        IsLoading = true;
        Results.Clear();

        try
        {
            var service = _factory.Create();
            var flights = await service.SearchFlightsAsync(Origin, Destination, DepartureDate);

            foreach (var f in flights)
                Results.Add(f);

            SearchCompleted?.Invoke(Results);

            if (Results.Count == 0)
                ErrorMessage = "No flights found. Supported routes: SOF↔LHR, SOF↔TXL, SOF↔CDG, SOF↔FCO, SOF↔AMS, SOF↔MXP.";
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"Network error: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Search failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
