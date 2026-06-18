using System.Windows.Controls;
using SkySaver.ViewModels;

namespace SkySaver.Views;

public partial class SettingsView : UserControl
{
    public SettingsView() => InitializeComponent();

    private void ApiKeyBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel vm)
            vm.SetApiKey(ApiKeyBox.Password);
    }

    private void ApiKeyBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel vm)
            ApiKeyBox.Password = vm.AviationStackApiKey;
    }
}
