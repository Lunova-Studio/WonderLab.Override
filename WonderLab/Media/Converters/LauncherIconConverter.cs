using Avalonia.Data.Converters;
using System;
using System.Globalization;
using WonderLab.Services;

namespace WonderLab.Media.Converters;

public sealed class LauncherIconConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is string iconName) {
            return iconName switch {
                "PCL" => ThemeService.PCLIcon.Value,
                "HMCL" => ThemeService.HMCLIcon.Value,
                _ => ThemeService.LoadingIcon.Value
            };
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
