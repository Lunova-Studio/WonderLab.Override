using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.IO;
using System;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace WonderLab.Extensions;

public static class AvaloniaExtension {
    [Obsolete("Avalonia has fixed this bug.")]
    public static AppBuilder WithFont(this AppBuilder app) {
        return app.With(new FontManagerOptions {
            DefaultFamilyName = "resm:WonderLab.Assets.Font.DinPro.ttf?assembly=WonderLab#DIN Pro",
            FontFallbacks = [new FontFallback { FontFamily = "Microsoft YaHei UI" }]
        });
    }

    public static byte[] ToBytes(this string uri) {
        var memoryStream = new MemoryStream();
        using var stream = AssetLoader.Open(new Uri(uri));
        stream!.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return memoryStream.ToArray();
    }

    public static Bitmap ToBitmap(this string uri) {
        var memoryStream = new MemoryStream();
        using var stream = AssetLoader.Open(new Uri(uri));
        stream!.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return new Bitmap(memoryStream);
    }

    public static Bitmap ToBitmap(this byte[] bytes) {
        return new Bitmap(new MemoryStream(bytes));
    }

    public static Bitmap ToBitmap<TPixel>(this Image<TPixel> raw) where TPixel : unmanaged, IPixel<TPixel> {
        using var stream = new MemoryStream();
        raw.Save(stream, new PngEncoder());
        stream.Position = 0;
        return new Bitmap(stream);
    }

    public static void SyncPost(this Dispatcher dispatcher, Action action) {
        dispatcher.SyncPost(DispatcherPriority.Normal, action);
    }

    public static async void SyncPost(this Dispatcher dispatcher, DispatcherPriority priority, Action action) {
        bool isDone = false;

        dispatcher.Post(() => {
            action.Invoke();
            isDone = true;
        }, priority);

        while (!isDone) {
            await Task.Delay(75);
        }
    }
}


//public static void SynchronousTryEnqueue(this DispatcherQueue dispatcher, DispatcherQueueHandler callback)
//    => dispatcher.SynchronousTryEnqueue(DispatcherQueuePriority.Normal, callback);

//public static void SynchronousTryEnqueue(this DispatcherQueue dispatcher, DispatcherQueuePriority priority, DispatcherQueueHandler callback) {
//    bool taskDone = false;

//    dispatcher.TryEnqueue(priority, () =>
//    {
//        callback.Invoke();
//        taskDone = true;
//    });

//    while (!taskDone)
//        Task.Delay(75).Wait();
//}