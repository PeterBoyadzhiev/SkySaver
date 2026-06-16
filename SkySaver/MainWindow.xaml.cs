using SkySaver.ViewModels;
using System.Windows;

namespace SkySaver;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
