using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.IO;
using System;

namespace WonderLab.Extensions;

public static class AvaloniaExtension {
    public static AppBuilder WithFont(this AppBuilder app) {
        return app.With(new FontManagerOptions {
            DefaultFamilyName = "resm:WonderLab.Assets.Font.DinPro.ttf?assembly=WonderLab#DIN Pro",
            FontFallbacks = [new FontFallback { FontFamily = "Microsoft YaHei UI" }]
        });
    }

    public static Bitmap ToBitmap(this string uri) {
        var memoryStream = new MemoryStream();
        using var stream = AssetLoader.Open(new Uri(uri));
        stream!.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return new Bitmap(memoryStream);
    }
}