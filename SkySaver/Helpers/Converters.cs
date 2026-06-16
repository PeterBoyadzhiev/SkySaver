using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SkySaver.Helpers;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is true ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type t, object p, CultureInfo c)
        => value is Visibility.Visible;
}

[ValueConversion(typeof(bool), typeof(Visibility))]
public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is false ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type t, object p, CultureInfo c)
        => value is not Visibility.Visible;
}

[ValueConversion(typeof(string), typeof(Visibility))]
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type t, object p, CultureInfo c)
        => throw new NotImplementedException();
}

[ValueConversion(typeof(int), typeof(Visibility))]
public class ZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is int i && i == 0 ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type t, object p, CultureInfo c)
        => throw new NotImplementedException();
}

[ValueConversion(typeof(object), typeof(Visibility))]
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value != null ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type t, object p, CultureInfo c)
        => throw new NotImplementedException();
}

[ValueConversion(typeof(bool), typeof(string))]
public class BoolToToggleTextConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is true ? "Pause" : "Resume";
    public object ConvertBack(object value, Type t, object p, CultureInfo c)
        => throw new NotImplementedException();
}
