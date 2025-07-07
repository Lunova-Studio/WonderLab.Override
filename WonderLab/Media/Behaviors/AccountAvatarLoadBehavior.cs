using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Base.Models.Authentication;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.SourceGenerator.Attributes;
using WonderLab.Utilities;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(Account), "Account")]
public sealed partial class AccountAvatarLoadBehavior : Behavior<Border> {
    private ILogger<AccountAvatarLoadBehavior> _logger;
    private CancellationTokenSource _cancellationTokenSource;

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null)
            return;

        _cancellationTokenSource = new();
        _logger = App.Get<ILogger<AccountAvatarLoadBehavior>>();

        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (AssociatedObject is null)
            return;

        using (_cancellationTokenSource) {
            _cancellationTokenSource.Cancel();
            AssociatedObject.Loaded -= OnLoaded;
        }
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) {
        if (Account is null)
            return;

        _logger.LogInformation("加载账户 {account} 的头像", Account.Name);
        if (SkinUtil.SkinAvatarCaches.TryGetValue(Account.Uuid, out var avatar))
            AssociatedObject.Background = avatar;
        else
            await LoadAvatarAsync();

        async Task LoadAvatarAsync() {
            var skinData = await SkinUtil
                .GetSkinDataAsync(Account, _cancellationTokenSource.Token);

            Dispatcher.UIThread.Post(() => {
                var avatar = SkinUtil.CroppedSkinAvatar(skinData);
                var brush = new ImageBrush(avatar).ToImmutable();

                //Why does null trigger here??
                if (Account is null)
                    return;

                SkinUtil.SkinAvatarCaches.Add(Account.Uuid, brush);
                AssociatedObject.Background = brush;
            });
        }
    }
}