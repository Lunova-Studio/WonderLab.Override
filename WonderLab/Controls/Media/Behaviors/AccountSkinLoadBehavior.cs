using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Skin;
using MinecraftLaunch.Skin.Class.Fetchers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.Infrastructure.Enums;

namespace WonderLab.Controls.Media.Behaviors;

public sealed class AccountSkinLoadBehavior : Behavior<Border> {
    readonly record struct AccountSkinCache {
        public static Dictionary<Account, IImmutableBrush> SkinAreas { get; }
            = new(new AccountEqualityComparer());

        public static void Add(Account account, IImmutableBrush area) {
            if (account is null || SkinAreas.ContainsKey(account)) {
                return;
            }

            SkinAreas.Add(account, area);
        }

        public static bool TryGetArea(Account account, out IImmutableBrush area) {
            if (SkinAreas.TryGetValue(account, out var area1)) {
                area = area1;
                return true;
            }

            area = null;
            return false;
        }
    }

    sealed class AccountEqualityComparer : IEqualityComparer<Account> {
        public bool Equals(Account account1, Account account2) {
            return account1.Type == account2.Type
                && account1.Name == account2.Name
                && account1.Uuid == account2.Uuid;
        }

        public int GetHashCode([DisallowNull] Account obj) {
            return obj.GetHashCode();
        }
    }

    public static readonly StyledProperty<SkinArea> SkinAreaProperty =
        AvaloniaProperty.Register<AccountSkinLoadBehavior, SkinArea>(nameof(SkinArea), SkinArea.Head);

    public static readonly StyledProperty<Account> AccountProperty =
        AvaloniaProperty.Register<AccountSkinLoadBehavior, Account>(nameof(Account));

    public SkinArea SkinArea {
        get => GetValue(SkinAreaProperty);
        set => SetValue(SkinAreaProperty, value);
    }

    public Account Account {
        get => GetValue(AccountProperty);
        set => SetValue(AccountProperty, value);
    }

    private async Task<byte[]> GetSkinAsync(Account account) {
        if (!NetworkInterface.GetIsNetworkAvailable()) {
            return "resm:WonderLab.Assets.Image.steve.png".ToBytes();
        }

        var skin = await Task.Run(async () => {
            return account.Type switch {
                AccountType.Microsoft => await new MicrosoftSkinFetcher(account.Uuid.ToString()).GetSkinAsync(),
                AccountType.Yggdrasil => await new YggdrasilSkinFetcher((account as YggdrasilAccount).YggdrasilServerUrl, account.Uuid.ToString()).GetSkinAsync(),
                AccountType.Offline => "resm:WonderLab.Assets.Image.steve.png".ToBytes(),
                _ => "resm:WonderLab.Assets.Image.steve.png".ToBytes()
            };
        });

        return skin;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) {
        var border = AssociatedObject ?? throw new Exception();

        if (AccountSkinCache.TryGetArea(Account, out var area)) {
            border.Background = area;
            return;
        }

        var skin = new SkinResolver(await GetSkinAsync(Account));
        await Dispatcher.UIThread.InvokeAsync(() => {
            var brush = new ImageBrush(skin.CropSkinHeadBitmap().ToBitmap())
                .ToImmutable();

            border.Background = brush;
            AccountSkinCache.Add(Account, brush);
        }, DispatcherPriority.Background);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) { }

    protected override void OnAttached() {
        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnloaded;
    }
}