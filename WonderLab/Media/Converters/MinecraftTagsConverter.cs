using Avalonia.Data.Converters;
using MinecraftLaunch.Base.Models.Game;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WonderLab.Media.Converters;

public sealed class MinecraftTagsConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is MinecraftEntry minecraft) {
            List<string> tags = [
                minecraft.Version.VersionId,
                minecraft.Version.Type.ToString(),
            ];

            if (minecraft is ModifiedMinecraftEntry modifiedMinecraft) {
                var loader = modifiedMinecraft.ModLoaders.FirstOrDefault();
                tags.Add($"{loader.Type} {loader.Version}");
            }

            return tags;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}