using Avalonia.Platform;
using SkiaSharp;
using System;
using System.IO;

namespace WonderLab.Extensions;

public static class SkiaExtension {
    public static SKBitmap ToSKBitmap(this byte[] bytes) {
        return SKBitmap.Decode(bytes);
    }


    public static Stream ToStream(this string uri) {
        var memoryStream = new MemoryStream();
        using var stream = AssetLoader.Open(new Uri(uri));
        stream!.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public static byte[] ToBytes(this string uri) {
        using var memoryStream = new MemoryStream();
        using var stream = AssetLoader.Open(new Uri(uri));
        stream!.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return memoryStream.ToArray();
    }

    public static void Save(this SKBitmap bitmap, string file) {
        using var temp = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.Create(file);
        temp.AsStream().CopyTo(stream);
    }
}