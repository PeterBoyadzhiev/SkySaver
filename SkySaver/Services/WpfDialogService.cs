using System.Windows;

namespace SkySaver.Services;

public class WpfDialogService : IDialogService
{
    public bool Confirm(string title, string message)
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }
}
