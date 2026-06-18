using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using SkySaver.Helpers;
using SkySaver.Models;
using SkySaver.Repositories;
using SkySaver.Services;

namespace SkySaver.ViewModels;

public class AlertsViewModel : ViewModelBase
{
    private readonly IPriceAlertRepository _repo;
    private readonly IDialogService _dialogService;
    private bool _isLoading;

    public AlertsViewModel(IPriceAlertRepository repo, IDialogService dialogService)
    {
        _repo = repo;
        _dialogService = dialogService;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        DeleteCommand = new AsyncRelayCommand<PriceAlert>(DeleteAsync);
        ToggleActiveCommand = new AsyncRelayCommand<PriceAlert>(ToggleActiveAsync);
    }

    public ObservableCollection<PriceAlert> Alerts { get; } = new ObservableCollection<PriceAlert>();

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
        catch (Exception ex)
        {
            try
            {
                var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SkySaver");
                Directory.CreateDirectory(logDir);
                File.AppendAllText(Path.Combine(logDir, "error.log"), $"[{DateTime.UtcNow:O}] AlertsViewModel.LoadAsync error: {ex}\n");
                try
                {
                    var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    File.AppendAllText(Path.Combine(desktop, "SkySaver-error.log"), $"[{DateTime.UtcNow:O}] AlertsViewModel.LoadAsync error: {ex}\n");
                }
                catch { }
            }
            catch { }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteAsync(PriceAlert? alert)
    {
        if (alert == null) return;
        if (!_dialogService.Confirm("Delete Alert", "Are you sure you want to delete this alert? This cannot be undone."))
            return;
        await _repo.DeleteAsync(alert.Id);
        Alerts.Remove(alert);
    }

    private async Task ToggleActiveAsync(PriceAlert? alert)
    {
        if (alert == null) return;
        alert.IsActive = !alert.IsActive;
        await _repo.UpdateAsync(alert);
    }
}
