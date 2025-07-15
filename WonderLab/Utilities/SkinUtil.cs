using Avalonia.Media;
using Flurl.Http;
using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Extensions;

namespace WonderLab.Utilities;

public static class SkinUtil {
    public static Dictionary<Guid, IImmutableBrush> SkinAvatarCaches { get; } = [];

    public static async Task<byte[]> GetSkinDataAsync(Account account, CancellationToken cancellationToken = default) {
        return await Task.Run(async () => {
            return account switch {
                OfflineAccount => "resm:WonderLab.Assets.gawrgura-13490790.png".ToBytes(),
                MicrosoftAccount mAccount => await GetMicrosoftSkinDataAsync(mAccount, cancellationToken),
                YggdrasilAccount yAccount => await GetYggdrasilSkinDataAsync(yAccount, cancellationToken),
                _ => throw new NotSupportedException()
            };
        }, cancellationToken);
    }

    #region Privates

    private static readonly string YggdrasilSplitUrl = "{0}/sessionserver/session/minecraft/profile/{1}";
    private static readonly string MicrosoftSplitUrl = "https://sessionserver.mojang.com/session/minecraft/profile/{0}";

    private static Task<byte[]> GetYggdrasilSkinDataAsync(YggdrasilAccount account, CancellationToken cancellationToken) {
        var url = string.Format(YggdrasilSplitUrl, account.YggdrasilServerUrl,
            account.Uuid.ToString("N"));

        return GetSkinDataAsync(url, cancellationToken);
    }

    private static Task<byte[]> GetMicrosoftSkinDataAsync(MicrosoftAccount account, CancellationToken cancellationToken) {
        var url = string.Format(MicrosoftSplitUrl, account.Uuid.ToString("N"));
        return GetSkinDataAsync(url, cancellationToken);
    }

    private static async Task<byte[]> GetSkinDataAsync(string url, CancellationToken cancellationToken) {
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

        return await HttpUtil.Request(skinUrl).GetBytesAsync(cancellationToken: cancellationToken);
    }

    private static SKBitmap ResizeImage(SKBitmap source, int width, int height) {
        var resizedBitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(resizedBitmap);

        canvas.DrawBitmap(source, new SKRect(0, 0, width, height));
        return resizedBitmap;
    }

    #endregion
}