using System;
using System.Globalization;
using System.Windows.Data;
using Xcommunity.Launcher.Gui.ViewModels;

namespace Xcommunity.Launcher.Gui.Resources;

public class StateToViewModelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            MainWindowState.Mods => ViewModelLocator.Instance.Mods,
            MainWindowState.Profiles => ViewModelLocator.Instance.Profiles,
            _ => Binding.DoNothing
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}