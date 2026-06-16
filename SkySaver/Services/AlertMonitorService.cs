using SkySaver.Repositories;

namespace SkySaver.Services;

public class AlertMonitorService
{
    private readonly IPriceAlertRepository _repo;
    private readonly IFlightSearchService _flightService;
    private readonly INotificationService _notifier;
    private readonly PeriodicTimer _timer;
    private CancellationTokenSource? _cts;

    public AlertMonitorService(
        IPriceAlertRepository repo,
        IFlightSearchService flightService,
        INotificationService notifier,
        TimeSpan? interval = null)
    {
        _repo = repo;
        _flightService = flightService;
        _notifier = notifier;
        _timer = new PeriodicTimer(interval ?? TimeSpan.FromMinutes(30));
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();
        _ = RunLoopAsync(_cts.Token);
    }

    public void Stop() => _cts?.Cancel();

    private async Task RunLoopAsync(CancellationToken ct)
    {
        // Run once immediately on start, then on each tick
        await CheckAllAlertsAsync();
        try
        {
            while (await _timer.WaitForNextTickAsync(ct))
                await CheckAllAlertsAsync();
        }
        catch (OperationCanceledException) { }
    }

    private async Task CheckAllAlertsAsync()
    {
        var alerts = await _repo.GetActiveAsync();
        foreach (var alert in alerts)
        {
            try
            {
                var price = await _flightService.GetLowestPriceAsync(alert.Origin, alert.Destination, alert.DepartureDate);
                if (price == null) continue;

                alert.LastCheckedPrice = price;
                alert.LastCheckedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(alert);

                if (price <= alert.TargetPrice)
                    _notifier.ShowPriceDrop(alert.DisplayRoute, alert.TargetPrice, price.Value, alert.Currency);
            }
            catch
            {
                // Don't crash the monitor loop if one alert fails
            }
        }
    }
}
