using System.Net.Http;
using SkySaver.Config;

namespace SkySaver.Services;

public class FlightServiceFactory : IFlightServiceFactory
{
    private readonly AppConfig _config;
    private readonly HttpClient _http;
    private readonly MockFlightService _mock;

    public FlightServiceFactory(AppConfig config, HttpClient http)
    {
        _config = config;
        _http   = http;
        _mock   = new MockFlightService();
    }

    public IFlightService Create()
    {
        if (_config.DataSourceMode == DataSourceMode.AviationStack
            && !string.IsNullOrWhiteSpace(_config.AviationStackApiKey))
        {
            _config.LastUsedSource = "Live API (Aviationstack)";
            var live = new AviationStackFlightService(_http, _config.AviationStackApiKey);
            return new FallbackFlightService(live, _mock);
        }

        _config.LastUsedSource = "Mock data (offline)";
        return _mock;
    }
}
