using Avalonia.Interactivity;
using WonderLab.Controls;
using WonderLab.ViewModels.Windows;

namespace WonderLab;

public sealed partial class MinecraftLogWindow : WonderWindow {
    public MinecraftLogWindow() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        var vm = DataContext as MinecraftLogWindowViewModel;
        vm.ScrollToEnd = PART_ScrollViewer.ScrollToEnd;
    }
}