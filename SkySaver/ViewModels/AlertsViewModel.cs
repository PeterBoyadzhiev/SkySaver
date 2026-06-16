using System.Collections.ObjectModel;
using System.Windows.Input;
using SkySaver.Helpers;
using SkySaver.Models;
using SkySaver.Repositories;

namespace SkySaver.ViewModels;

public class AlertsViewModel : ViewModelBase
{
    private readonly IPriceAlertRepository _repo;
    private bool _isLoading;

    public AlertsViewModel(IPriceAlertRepository repo)
    {
        _repo = repo;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        DeleteCommand = new AsyncRelayCommand<PriceAlert>(DeleteAsync);
        ToggleActiveCommand = new AsyncRelayCommand<PriceAlert>(ToggleActiveAsync);
    }

    public ObservableCollection<PriceAlert> Alerts { get; } = [];

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ToggleActiveCommand { get; }

    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var alerts = await _repo.GetAllAsync();
            Alerts.Clear();
            foreach (var a in alerts)
                Alerts.Add(a);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteAsync(PriceAlert? alert)
    {
        if (alert == null) return;
        await _repo.DeleteAsync(alert.Id);
        Alerts.Remove(alert);
    }

    private async Task ToggleActiveAsync(PriceAlert? alert)
    {
        if (alert == null) return;
        alert.IsActive = !alert.IsActive;
        await _repo.UpdateAsync(alert);
        OnPropertyChanged(nameof(Alerts));
    }
}
