using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Extensions;

public static class AvaloniaExtsnsion {
    public static Bitmap ToBitmap(this string uri) {
        using var stream = AssetLoader.Open(new Uri(uri));
        return new Bitmap(stream);
    }

    public static async Task<string> ToTextAsync(this Uri uri, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(uri);

        using var stream = AssetLoader.Open(uri) ?? throw new InvalidOperationException($"Asset not found: {uri}");
        using var reader = new StreamReader(stream, Encoding.UTF8, true, 81920);
        return await reader.ReadToEndAsync(cancellationToken)
            .WaitAsync(cancellationToken);
    }
}