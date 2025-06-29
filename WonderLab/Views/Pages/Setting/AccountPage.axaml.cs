using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Authentication;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Controls;
using WonderLab.Services.Authentication;

namespace WonderLab;

public partial class AccountPage : Page {
    public AccountPage() {
        InitializeComponent();
    }

    private void OnAccountCardLoaded(object sender, RoutedEventArgs e) {
        var tile = (sender as Tile)!;
        var account = (tile.DataContext as Account)!;
        var activeAccount = App.Get<AccountService>().ActiveAccount;
        var activeTileBorderBrush = this.FindResource("TileSelectedBorderBrush") as SolidColorBrush;

        WeakReferenceMessenger.Default.Register<ActiveAccountChangedMessage>(sender, (r, m) => {
            tile!.BorderBrush = account?.Uuid == App.Get<AccountService>().ActiveAccount?.Uuid
                ? activeTileBorderBrush
                : tile.Background;
        });

        tile!.BorderBrush = account?.Uuid == App.Get<AccountService>().ActiveAccount?.Uuid
            ? activeTileBorderBrush
            : tile.Background;

        tile.Unloaded += (s, e) => WeakReferenceMessenger.Default.Unregister<ActiveAccountChangedMessage>(sender);
    }
}