using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.Interfaces.Navigation;
using WonderLab.ViewModels.Pages;

namespace WonderLab.Views.Windows;

public partial class MainWindow : Window {
    public MainWindow(INavigationService nav) {
        InitializeComponent();

        nav.Attach(RootNav);
        nav.NavigateToAsync<HomePageViewModel>();
    }

#if !DEBUG
    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        TestTipBorder.IsVisible = true;
    }
#endif

    private void Button_Click(object? sender, RoutedEventArgs e) {
        TestTipBorder.IsVisible = false;
    }
}