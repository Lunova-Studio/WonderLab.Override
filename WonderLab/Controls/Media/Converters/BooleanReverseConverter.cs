using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WonderLab.Controls.Media.Converters;

public sealed class BooleanReverseConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return !System.Convert.ToBoolean(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}