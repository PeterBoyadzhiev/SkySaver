using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using SkySaver.Data;
using SkySaver.Repositories;
using SkySaver.Services;
using SkySaver.ViewModels;
using SkySaver.Views;
using System.Windows;

namespace SkySaver;

public partial class App : Application
{
    private ServiceProvider? _services;
    private AlertMonitorService? _monitor;

    private void OnStartup(object sender, StartupEventArgs e)
    {
        DatabaseInitializer.Initialize();

        var services = new ServiceCollection();

        // API credentials — replace with your own from https://developers.amadeus.com
        const string clientId     = "YOUR_AMADEUS_CLIENT_ID";
        const string clientSecret = "YOUR_AMADEUS_CLIENT_SECRET";

        services.AddSingleton<HttpClient>();
        services.AddSingleton<IFlightSearchService>(sp =>
            new AmadeusFlightSearchService(sp.GetRequiredService<HttpClient>(), clientId, clientSecret));

        services.AddSingleton<IPriceAlertRepository, PriceAlertRepository>();
        services.AddSingleton<INotificationService, WindowsNotificationService>();
        services.AddSingleton<AlertMonitorService>();

        services.AddTransient<SearchViewModel>();
        services.AddTransient<ResultsViewModel>();
        services.AddTransient<AlertsViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddTransient<SearchView>();
        services.AddTransient<ResultsView>();
        services.AddTransient<AlertsView>();
        services.AddSingleton<MainWindow>();

        _services = services.BuildServiceProvider();

        _monitor = _services.GetRequiredService<AlertMonitorService>();
        _monitor.Start();

        var window = _services.GetRequiredService<MainWindow>();
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _monitor?.Stop();
        _services?.Dispose();
        base.OnExit(e);
    }
}
