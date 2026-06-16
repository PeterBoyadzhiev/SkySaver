using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SkySaver.Config;
using SkySaver.Data;
using SkySaver.Repositories;
using SkySaver.Services;
using SkySaver.ViewModels;
using SkySaver.Views;

namespace SkySaver;

public partial class App : Application
{
    private ServiceProvider? _services;
    private AlertMonitorService? _monitor;

    private void OnStartup(object sender, StartupEventArgs e)
    {
        DatabaseInitializer.Initialize();

        var appConfig = new AppConfig();
        TryLoadApiKeyFromSettings(appConfig);

        var services = new ServiceCollection();

        services.AddSingleton(appConfig);
        services.AddSingleton<HttpClient>(_ =>
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            return client;
        });

        services.AddSingleton<IFlightServiceFactory, FlightServiceFactory>();

        services.AddSingleton<IPriceAlertRepository, PriceAlertRepository>();
        services.AddSingleton<INotificationService, WindowsNotificationService>();
        services.AddSingleton<AlertMonitorService>();

        services.AddTransient<SearchViewModel>();
        services.AddTransient<ResultsViewModel>();
        services.AddTransient<AlertsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddTransient<SearchView>();
        services.AddTransient<ResultsView>();
        services.AddTransient<AlertsView>();
        services.AddTransient<SettingsView>();
        services.AddSingleton<MainWindow>();

        _services = services.BuildServiceProvider();

        _monitor = _services.GetRequiredService<AlertMonitorService>();
        _monitor.Start();

        _services.GetRequiredService<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _monitor?.Stop();
        _services?.Dispose();
        base.OnExit(e);
    }

    private static void TryLoadApiKeyFromSettings(AppConfig config)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        if (!File.Exists(path)) return;
        try
        {
            var json = File.ReadAllText(path);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("AviationStackApiKey", out var el))
                config.AviationStackApiKey = el.GetString() ?? string.Empty;
        }
        catch { }
    }
}
