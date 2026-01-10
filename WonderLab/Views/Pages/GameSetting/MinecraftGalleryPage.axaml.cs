using Avalonia.Controls;
using Avalonia.Interactivity;
using WonderLab.ViewModels.Pages.GameSetting;

namespace WonderLab.Views.Pages.GameSetting;

public sealed partial class MinecraftGalleryPage : UserControl {
    private MinecraftGalleryPageViewModel VM;

    public MinecraftGalleryPage() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        VM = DataContext as MinecraftGalleryPageViewModel;
    }

    protected override void OnUnloaded(RoutedEventArgs e) {
        base.OnUnloaded(e);
        VM.IsActive = false;
    }
}