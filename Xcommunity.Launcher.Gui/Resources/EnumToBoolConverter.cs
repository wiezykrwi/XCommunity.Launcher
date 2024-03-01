using System;
using System.Windows.Data;

namespace Xcommunity.Launcher.Gui.Resources;

public class EnumToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value?.Equals(parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return (value, parameter) switch
        {
            (true, not null) => parameter,
            _ => Binding.DoNothing
        };
    }
}
