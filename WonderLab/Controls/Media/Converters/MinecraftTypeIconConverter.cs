using Avalonia.Data.Converters;
using System;
using System.Globalization;
using WonderLab.Infrastructure.Models.Launch;
using WonderLab.Services.UI;

namespace WonderLab.Controls.Media.Converters;

public sealed class MinecraftTypeIconConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is GameModel) {
            var model = value as GameModel;
            return model.Entry.IsVanilla ? model.Entry.Type switch {
                "old_beta" => ThemeService.OldMinecraftIcon.Value,
                "old_alpha" => ThemeService.OldMinecraftIcon.Value,
                "release" => ThemeService.ReleaseMinecraftIcon.Value,
                "snapshot" => ThemeService.SnapshotMinecraftIcon.Value,
                _ => null
            } : ThemeService.LoaderMinecraftIcon.Value;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}