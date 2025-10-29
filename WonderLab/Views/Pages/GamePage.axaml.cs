using Avalonia.Interactivity;
using System.Threading.Tasks;
using WonderLab.Controls;

namespace WonderLab.Views.Pages;

public partial class GamePage : Page {
    public GamePage() {
        InitializeComponent();
    }

    protected async override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        await Task.Delay(200);
        Tile.RunParentPanelAnimation(PART_ItemsRepeater, true);
    }
}