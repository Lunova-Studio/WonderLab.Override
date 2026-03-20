using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.Controls;

namespace WonderLab;

public partial class TestWindow : WonderWindow {
    public TestWindow() {
        InitializeComponent();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        var pageKey = (e.AddedItems[0] as Control).Tag
            .ToString();

        PART_TaskBar.TaskBarState = pageKey is "Home"
            ? TaskBarState.Expand
            : TaskBarState.Hidden;

        PART_NavigationRail.Frame.Navigate(pageKey);
    }

    private void OnNavigationItemChecked(object sender, RoutedEventArgs e) {
        var control = sender as NavigationItem;
        if (PART_NavigationRail.IsLoaded && control.IsChecked.Value)
            PART_NavigationRail.Frame.Navigate(control.Tag.ToString());
    }
}