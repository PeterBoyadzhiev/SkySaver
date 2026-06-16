using System.Collections.ObjectModel;
using System.Windows.Input;
using SkySaver.Helpers;
using SkySaver.Models;
using SkySaver.Repositories;

namespace SkySaver.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    private readonly IPriceAlertRepository _repo;
    private string _statusMessage = string.Empty;

    public ResultsViewModel(IPriceAlertRepository repo)
    {
        _repo = repo;
        SaveAlertCommand = new AsyncRelayCommand<Flight>(SaveAlertAsync);
    }

    public ObservableCollection<Flight> Flights { get; } = [];

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SaveAlertCommand { get; }

    public void LoadResults(IEnumerable<Flight> flights)
    {
        Flights.Clear();
        foreach (var f in flights)
            Flights.Add(f);
        StatusMessage = string.Empty;
    }

    private async Task SaveAlertAsync(Flight? flight)
    {
        if (flight == null) return;

        var alert = new PriceAlert
        {
            Origin = flight.Origin,
            Destination = flight.Destination,
            DepartureDate = flight.DepartureTime.Date,
            TargetPrice = flight.Price,
            Currency = flight.Currency,
        };

        await _repo.AddAsync(alert);
        StatusMessage = $"Alert saved! You'll be notified if {flight.Origin}→{flight.Destination} drops below {flight.Price:F2} {flight.Currency}.";
    }
}

// Generic async relay command for parameterized actions
public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T?, Task> _execute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<T?, Task> execute) => _execute = execute;

    public event EventHandler? CanExecuteChanged
    {
        add => System.Windows.Input.CommandManager.RequerySuggested += value;
        remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => !_isExecuting;

    public async void Execute(object? parameter)
    {
        _isExecuting = true;
        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        try { await _execute(parameter is T t ? t : default); }
        finally
        {
            _isExecuting = false;
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }
    }
}
