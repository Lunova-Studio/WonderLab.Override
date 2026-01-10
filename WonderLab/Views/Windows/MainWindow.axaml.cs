using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using WonderLab.Controls;
using WonderLab.Models;
using WonderLab.Override;
using WonderLab.ViewModels.Windows;

namespace WonderLab.Views.Windows;

public partial class MainWindow : WonderWindow {
    private bool _isSync = false;
    private MainWindowViewModel _vm = null;

    public MainWindow() {
        InitializeComponent();
    }

    private void OnNavigated() {
        PART_BackButton.IsEnabled = PART_HostFrame.CanGoBack;
        PART_ForwardButton.IsEnabled = PART_HostFrame.CanGoForward;
    }

    private void SyncTabSelection() {
        _isSync = true;
        PART_ListBox.SelectedItem = _vm.TabItems
            .FirstOrDefault(x => x.PageKey == PART_HostFrame.PageKey);

        OnNavigated();
        _isSync = false;
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e) {
        PART_HostFrame.GoBack();
        SyncTabSelection();
    }

    private void OnForwardButtonClick(object sender, RoutedEventArgs e) {
        PART_HostFrame.GoForward();
        SyncTabSelection();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (PART_ListBox is null || _isSync)
            return;

        if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItemModel tabItem) {
            if (tabItem.PageKey.Contains("Setting", StringComparison.InvariantCultureIgnoreCase)) {
                PART_ListBox.SelectedIndex = 0;

                _ = new SettingsWindow() {
                    DataContext = App.Get<SettingsWindowViewModel>()
                }.ShowDialog(this);
            } else {
                PART_HostFrame.Navigate(tabItem.PageKey);
            }
        }

        OnNavigated();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        _vm = DataContext as MainWindowViewModel;
#if DEBUG
        PART_TestTipBanner.IsVisible = false;
#endif
    }
}