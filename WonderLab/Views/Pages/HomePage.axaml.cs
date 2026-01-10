using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.ViewModels.Pages;

namespace WonderLab.Views.Pages;

public partial class HomePage : UserControl {
    private HomePageViewModel VM;

    public HomePage() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        VM = DataContext as HomePageViewModel;
    }

    protected override void OnUnloaded(RoutedEventArgs e) {
        base.OnUnloaded(e);
        VM.IsActive = false;
    }
}