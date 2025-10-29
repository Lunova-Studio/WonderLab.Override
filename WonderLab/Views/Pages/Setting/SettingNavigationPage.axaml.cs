using Avalonia.Interactivity;
using System.Threading.Tasks;
using WonderLab.Controls;

namespace WonderLab.Views.Pages.Setting;

public sealed partial class SettingNavigationPage : Page {
    public SettingNavigationPage() {
        InitializeComponent();
    }

    protected async override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        await Task.Delay(200);
        Tile.RunParentPanelAnimation(PART_TileListBox?.ItemsPanelRoot, true);
    }
}