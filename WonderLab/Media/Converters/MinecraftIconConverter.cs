using Avalonia.Data.Converters;
using MinecraftLaunch.Base.Models.Network;
using System;
using System.Globalization;
using WonderLab.Services;

namespace WonderLab.Media.Converters;
public sealed class MinecraftIconConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is not VersionManifestEntry entry)
            return null;

        return entry.Type switch {
            "old_beta" => ThemeService.OldMinecraftIcon.Value,
            "old_alpha" => ThemeService.OldMinecraftIcon.Value,
            "release" => ThemeService.ReleaseMinecraftIcon.Value,
            "snapshot" => ThemeService.SnapshotMinecraftIcon.Value,
            _ => null
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
