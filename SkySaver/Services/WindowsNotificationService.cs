using Microsoft.Toolkit.Uwp.Notifications;

namespace SkySaver.Services;

public class WindowsNotificationService : INotificationService
{
    public void ShowPriceDrop(string route, decimal targetPrice, decimal currentPrice, string currency)
    {
        new ToastContentBuilder()
            .AddAppLogoOverride(new Uri("ms-appx:///Assets/plane.png"), ToastGenericAppLogoCrop.Circle)
            .AddText("✈ Price Drop Alert – SkySaver")
            .AddText($"{route}")
            .AddText($"Now {currentPrice:F2} {currency} — your target was {targetPrice:F2} {currency}")
            .Show();
    }
}
