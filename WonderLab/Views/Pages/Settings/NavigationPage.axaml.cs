using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.Models;

namespace WonderLab;

public partial class NavigationPage : UserControl {
    public NavigationPage() {
        InitializeComponent();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (PART_ListBox is null)
            return;

        if (e.AddedItems.Count > 0 && e.AddedItems[0] is ListItemModel tabItem) {
            PART_Frame.Navigate(tabItem.PageKey);
        }
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        PART_ListBox.SelectedIndex = 0;
    }
}