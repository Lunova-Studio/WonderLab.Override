using Avalonia.Data.Converters;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Base.Interfaces;
using System;
using System.Globalization;
using WonderLab.Services;

namespace WonderLab.Media.Converters;

public sealed class LoaderIconConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is null)
            return null;

        var entry = value as IInstallEntry;
        return entry?.ModLoaderType switch {
            ModLoaderType.Forge => ThemeService.LoaderForgeIcon.Value,
            ModLoaderType.Quilt => ThemeService.LoaderQuiltIcon.Value,
            ModLoaderType.Fabric => ThemeService.LoaderFabricIcon.Value,
            ModLoaderType.OptiFine => ThemeService.LoaderOptifineIcon.Value,
            ModLoaderType.NeoForge => ThemeService.LoaderNeoforgeIcon.Value,
            _ => throw new NotSupportedException($"Unsupported loader type: {entry?.ModLoaderType}")
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}