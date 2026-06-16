namespace SkySaver.Services;

public interface INotificationService
{
    void ShowPriceDrop(string route, decimal targetPrice, decimal currentPrice, string currency);
}
