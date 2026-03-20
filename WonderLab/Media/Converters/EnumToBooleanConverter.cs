using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WonderLab.Media.Converters;

public sealed class EnumToBooleanConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value?.ToString() == parameter?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return (bool)value 
            ? Enum.Parse(targetType, parameter.ToString()) 
            : BindingValueType.DoNothing;
    }
}