using SkySaver.Helpers;

namespace SkySaver.Config;

public class AppConfig : ViewModelBase
{
    private DataSourceMode _dataSourceMode = DataSourceMode.Mock;
    private string _aviationStackApiKey = string.Empty;
    private string _lastUsedSource = string.Empty;

    public DataSourceMode DataSourceMode
    {
        get => _dataSourceMode;
        set => SetProperty(ref _dataSourceMode, value);
    }

    public string AviationStackApiKey
    {
        get => _aviationStackApiKey;
        set => SetProperty(ref _aviationStackApiKey, value);
    }

    /// <summary>
    /// Set by FlightServiceFactory.Create() to indicate which data source
    /// will actually serve results (accounts for fallback behaviour).
    /// </summary>
    public string LastUsedSource
    {
        get => _lastUsedSource;
        set => SetProperty(ref _lastUsedSource, value);
    }
}
