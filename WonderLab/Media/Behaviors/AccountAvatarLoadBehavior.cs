using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using LiteSkinViewer2D;
using LiteSkinViewer2D.Extensions;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Authentication;
using SkiaSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.SourceGenerator.Attributes;
using WonderLab.Utilities;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(Account), "Account")]
public sealed partial class AccountAvatarLoadBehavior : Behavior<Border> {
    private CancellationTokenSource _cts;
    private ILogger<AccountAvatarLoadBehavior> _logger;

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null)
            return;

        _logger = App.Get<ILogger<AccountAvatarLoadBehavior>>();

        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (AssociatedObject is null)
            return;

        AssociatedObject.Loaded -= OnLoaded;

    }

    private async void OnLoaded(object sender, RoutedEventArgs e) {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        if (Account == null)
            return;

        var snapshot = Account;
        var uuid = snapshot.Uuid;
        var name = snapshot.Name;

        _logger.LogInformation("加载账户 {account} 的头像", name);

        await Task.Delay(400);
        if (SkinUtil.SkinAvatarCaches.TryGetValue(uuid, out var brush)) {
            AssociatedObject.Background = brush;
            return;
        }

        _ = LoadAvatarAsync(snapshot, _cts.Token);
    }

    private async Task LoadAvatarAsync(Account snapshot, CancellationToken token) {
        try {
            var avaloniaBitmap = await Task.Run(async () => {
                var skinData = await SkinUtil.GetSkinDataAsync(snapshot, token);

                using var skBmp = SKBitmap.Decode(skinData);
                using var avatarSkBmp = HeadCapturer.Default.Capture(skBmp);
                return avatarSkBmp.ToBitmap();
            }, token);

            if (token.IsCancellationRequested)
                return;

            var finalBrush = new ImageBrush(avaloniaBitmap).ToImmutable();
            SkinUtil.SkinAvatarCaches[snapshot.Uuid] = finalBrush;
            AssociatedObject.Background = finalBrush;
        } catch (OperationCanceledException) { } catch (Exception ex) {
            _logger.LogError(ex, "加载头像失败：{user}", snapshot.Name);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) {
        _cts?.Cancel();
    }
}