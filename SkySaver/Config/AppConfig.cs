using SkySaver.Helpers;

namespace SkySaver.Config;

public class AppConfig : ViewModelBase
{
    private DataSourceMode _dataSourceMode = DataSourceMode.Mock;
    private string _aviationStackApiKey = string.Empty;

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
}
