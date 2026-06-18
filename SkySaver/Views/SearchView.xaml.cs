using System.Windows.Controls;
using SkySaver.ViewModels;

namespace SkySaver.Views;

public partial class SearchView : UserControl
{
    public SearchView() => InitializeComponent();

    private void RoundTripCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SearchViewModel vm)
            vm.ReturnDate = vm.DepartureDate.AddDays(7);
    }

    private void RoundTripCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SearchViewModel vm)
            vm.ReturnDate = null;
    }
}
