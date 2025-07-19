using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteSkinViewer2D;
using LiteSkinViewer2D.Extensions;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Authentication;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.Services;
using WonderLab.Utilities;

namespace WonderLab.Classes.Models;

public sealed partial class AccountModel : ObservableObject {
    private readonly ILogger _logger;

    [ObservableProperty] private Account _account;
    [ObservableProperty] private Bitmap _avatar = ThemeService.LoadingIcon.Value;

    public AccountModel(Account account, ILogger logger) {
        Account = account;
        _logger = logger;

        _ = LoadAvatarAsync();
    }

    private Task LoadAvatarAsync() => Task.Run(async () => {
        if (Account is null)
            return;

        try {
            if (SkinUtil.SkinAvatarCaches.TryGetValue(Account.Uuid, out var cachedAvatar)) {
                Avatar = cachedAvatar;
                return;
            }

            using var skinBitmap = await SkinUtil.GetSkinDataAsync(Account);
            using var avatarSK = HeadCapturer.Default.Capture(skinBitmap);

            Avatar = avatarSK.ToBitmap();
            SkinUtil.SkinAvatarCaches.TryAdd(Account.Uuid, Avatar);
        } catch (Exception) {
            _logger.LogError("Failed to load avatar for account {account}", Account.Name);
        }
    });
}