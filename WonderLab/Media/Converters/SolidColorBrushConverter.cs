using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace WonderLab.Media.Converters;

public sealed class SolidColorBrushConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is uint color)
            return new SolidColorBrush(Color.FromUInt32(color));

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
