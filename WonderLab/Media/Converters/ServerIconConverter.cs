using Avalonia.Data.Converters;
using Avalonia.Media;
using LiteSkinViewer2D.Extensions;
using System;
using System.Globalization;
using WonderLab.Extensions;

namespace WonderLab.Media.Converters;

public sealed class ServerIconConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var base64 = value.ToString();

        if (string.IsNullOrEmpty(base64))
            return null;

        return new ImageBrush(System.Convert.FromBase64String(base64)
            .ToSKBitmap().ToBitmap());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
