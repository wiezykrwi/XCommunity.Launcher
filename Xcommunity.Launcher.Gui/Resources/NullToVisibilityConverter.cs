using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Xcommunity.Launcher.Gui.Resources;

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value switch
        {
            not null => Visibility.Visible,
            _ => Visibility.Collapsed
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}