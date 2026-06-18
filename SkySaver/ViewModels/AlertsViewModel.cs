using System.Collections.ObjectModel;
using System.IO;
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
        RequestDeleteCommand = new AsyncRelayCommand<PriceAlert>(RequestDeleteAsync);
        ConfirmDeleteCommand = new AsyncRelayCommand<PriceAlert>(ConfirmDeleteAsync);
        CancelDeleteCommand = new AsyncRelayCommand<PriceAlert>(alert => { if (alert != null) alert.IsConfirmingDelete = false; return Task.CompletedTask; });
        ToggleActiveCommand = new AsyncRelayCommand<PriceAlert>(ToggleActiveAsync);
    }

    public ObservableCollection<PriceAlert> Alerts { get; } = new ObservableCollection<PriceAlert>();

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand RequestDeleteCommand { get; }
    public ICommand ConfirmDeleteCommand { get; }
    public ICommand CancelDeleteCommand { get; }
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

    // Step 1 — show inline confirmation UI; no repository call.
    private Task RequestDeleteAsync(PriceAlert? alert)
    {
        if (alert != null)
            alert.IsConfirmingDelete = true;
        return Task.CompletedTask;
    }

    // Step 2 — user confirmed; delete from repo and remove from collection.
    private async Task ConfirmDeleteAsync(PriceAlert? alert)
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
    }
}
