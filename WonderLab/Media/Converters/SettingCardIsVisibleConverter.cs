using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WonderLab.Media.Converters;

public sealed class SettingCardIsVisibleConverter : IMultiValueConverter {
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
        if (values[0] is bool boolValue && values[1] is object objectValue)
            return boolValue && objectValue is not null;

        return false;
    }
}