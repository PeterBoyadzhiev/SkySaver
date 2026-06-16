using System.Windows.Input;
using SkySaver.Helpers;

namespace SkySaver.ViewModels;

public class MainViewModel : ViewModelBase
{
    private object _currentView;

    public MainViewModel(
        SearchViewModel searchVm,
        ResultsViewModel resultsVm,
        AlertsViewModel alertsVm,
        SettingsViewModel settingsVm)
    {
        SearchVm   = searchVm;
        ResultsVm  = resultsVm;
        AlertsVm   = alertsVm;
        SettingsVm = settingsVm;

        _currentView = SearchVm;

        SearchVm.SearchCompleted += flights =>
        {
            ResultsVm.LoadResults(flights);
            CurrentView = ResultsVm;
        };

        NavigateSearchCommand   = new RelayCommand(() => CurrentView = SearchVm);
        NavigateResultsCommand  = new RelayCommand(() => CurrentView = ResultsVm);
        NavigateAlertsCommand   = new RelayCommand(async () =>
        {
            CurrentView = AlertsVm;
            await ((AsyncRelayCommand)AlertsVm.LoadCommand).ExecuteAsync();
        });
        NavigateSettingsCommand = new RelayCommand(() => CurrentView = SettingsVm);
    }

    public SearchViewModel   SearchVm   { get; }
    public ResultsViewModel  ResultsVm  { get; }
    public AlertsViewModel   AlertsVm   { get; }
    public SettingsViewModel SettingsVm { get; }

    public object CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public ICommand NavigateSearchCommand   { get; }
    public ICommand NavigateResultsCommand  { get; }
    public ICommand NavigateAlertsCommand   { get; }
    public ICommand NavigateSettingsCommand { get; }
}
