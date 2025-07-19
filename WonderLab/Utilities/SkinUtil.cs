using Avalonia.Media.Imaging;
using Flurl.Http;
using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Extensions;

namespace WonderLab.Utilities;

public static class SkinUtil {
    private static readonly string CacheFolderPath =
        Path.Combine(PathUtil.GetDataFolderPath(), "cache", "skin");

    public static Dictionary<Guid, SKBitmap> SkinCaches { get; } = [];
    public static Dictionary<Guid, Bitmap> SkinAvatarCaches { get; } = [];

    public static void InitCacheFolder() {
        if (Directory.Exists(CacheFolderPath))
            return;

        Directory.CreateDirectory(CacheFolderPath);
    }

    public static async Task<SKBitmap> GetSkinDataAsync(Account account, CancellationToken cancellationToken = default) {
        return await Task.Run(async () => {
            var cachePath = Path.Combine(CacheFolderPath, $"{account.Uuid:N}");
            if (File.Exists(cachePath))
                return SKBitmap.Decode(File.OpenRead(cachePath));

            var data = account switch {
                OfflineAccount => "resm:WonderLab.Assets.gawrgura-13490790.png".ToStream(),
                MicrosoftAccount mAccount => await GetMicrosoftSkinDataAsync(mAccount, cancellationToken),
                YggdrasilAccount yAccount => await GetYggdrasilSkinDataAsync(yAccount, cancellationToken),
                _ => throw new NotSupportedException()
            };

            var skinBitmap = SKBitmap.Decode(data);
            skinBitmap.Save(cachePath);

            return skinBitmap;
        }, cancellationToken);
    }

    #region Privates

    private static readonly string YggdrasilSplitUrl = "{0}/sessionserver/session/minecraft/profile/{1}";
    private static readonly string MicrosoftSplitUrl = "https://sessionserver.mojang.com/session/minecraft/profile/{0}";

    private static Task<Stream> GetYggdrasilSkinDataAsync(YggdrasilAccount account, CancellationToken cancellationToken) {
        var url = string.Format(YggdrasilSplitUrl, account.YggdrasilServerUrl,
            account.Uuid.ToString("N"));

        return GetSkinDataAsync(url, cancellationToken);
    }

    private static Task<Stream> GetMicrosoftSkinDataAsync(MicrosoftAccount account, CancellationToken cancellationToken) {
        var url = string.Format(MicrosoftSplitUrl, account.Uuid.ToString("N"));
        return GetSkinDataAsync(url, cancellationToken);
    }

    private static async Task<Stream> GetSkinDataAsync(string url, CancellationToken cancellationToken) {
        var baseJson = await HttpUtil.Request(url).GetStringAsync(cancellationToken: cancellationToken);
        var baseNode = baseJson?.AsNode();

        var base64 = baseNode?.GetEnumerable("properties")
            ?.FirstOrDefault()
            ?.GetString("value");

        var skinJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        var skinNode = skinJson.AsNode();

        var skinUrl = skinNode?.Select("textures")?
            .Select("SKIN")?
            .GetString("url");

        return await HttpUtil.Request(skinUrl).GetStreamAsync(cancellationToken: cancellationToken);
    }

    private static SKBitmap ResizeImage(SKBitmap source, int width, int height) {
        var resizedBitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(resizedBitmap);

        canvas.DrawBitmap(source, new SKRect(0, 0, width, height));
        return resizedBitmap;
    }

    #endregion
}