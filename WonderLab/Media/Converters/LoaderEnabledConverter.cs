using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WonderLab.Media.Converters;

public sealed class LoaderEnabledConverter : IMultiValueConverter {
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
        if (values?.Count <= 0 || values.Any(x => x is UnsetValueType))
            return false;

        var isOptional = (bool)values[1];
        var isSupported = (bool)values[0];

        return isSupported && isOptional;
    }
}