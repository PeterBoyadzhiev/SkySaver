using SkySaver.Config;
using SkySaver.Helpers;

namespace SkySaver.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly AppConfig _config;

    public SettingsViewModel(AppConfig config)
    {
        _config = config;
    }

    public bool IsMockMode
    {
        get => _config.DataSourceMode == DataSourceMode.Mock;
        set
        {
            if (value) _config.DataSourceMode = DataSourceMode.Mock;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsLiveMode));
            OnPropertyChanged(nameof(ModeLabel));
        }
    }

    public bool IsLiveMode
    {
        get => _config.DataSourceMode == DataSourceMode.AviationStack;
        set
        {
            if (value) _config.DataSourceMode = DataSourceMode.AviationStack;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsMockMode));
            OnPropertyChanged(nameof(ModeLabel));
        }
    }

    public string AviationStackApiKey
    {
        get => _config.AviationStackApiKey;
        set
        {
            _config.AviationStackApiKey = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Called from SettingsView code-behind when PasswordBox.Password changes.
    /// </summary>
    public void SetApiKey(string key)
    {
        _config.AviationStackApiKey = key;
        OnPropertyChanged(nameof(AviationStackApiKey));
    }

    public string ModeLabel => _config.DataSourceMode == DataSourceMode.Mock
        ? "Offline mode (Mock data)"
        : "Live API mode (Aviationstack)";
}
