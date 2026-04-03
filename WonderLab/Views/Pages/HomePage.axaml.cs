using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WonderLab.Interfaces.Navigation;

namespace WonderLab;

public partial class HomePage : UserControl {
    private INavigationService navigationService;

    public HomePage(INavigationService navigationService) {
        InitializeComponent();
        this.navigationService = navigationService;
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        navigationService.NavigateToPageAsync<MinecraftPage>();
    }
}