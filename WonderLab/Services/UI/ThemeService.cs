using System;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Media.Imaging;
using WonderLab.Extensions;
using Avalonia.Controls;
using Avalonia.Media;
using WonderLab.Views.Windows;
using MinecraftLaunch.Utilities;

namespace WonderLab.Services.UI;

public sealed class ThemeService {
    private readonly SettingService _settingService;
    private readonly string DINPro = "resm:WonderLab.Assets.Fonts.DinPro.ttf?assembly=WonderLab#DIN Pro";

    public static readonly Lazy<Bitmap> OldMinecraftIcon = new("resm:WonderLab.Assets.Images.Icons.old_minecraft.png".ToBitmap());
    public static readonly Lazy<Bitmap> ReleaseMinecraftIcon = new("resm:WonderLab.Assets.Images.Icons.release_minecraft.png".ToBitmap());
    public static readonly Lazy<Bitmap> SnapshotMinecraftIcon = new("resm:WonderLab.Assets.Images.Icons.snapshot_minecraft.png".ToBitmap());

    public ThemeService(SettingService settingService) { 
        _settingService = settingService;
    }

    public void ApplyBackgroundAfterPageLoad(MainWindow window) {
        window.Background = Brushes.Transparent;
        window.AcrylicMaterial.IsVisible = false;

        switch (_settingService.Data.BackgroundIndex) {
            case 0:
                window.Background = Application.Current.Resources["ColourBackground"] as IBrush;
                break;
            case 1:
                if (EnvironmentUtil.IsWindow) {
                    window.TransparencyLevelHint = [WindowTransparencyLevel.Mica];
                } else {
                    window.Background = Application.Current.Resources["ColourBackground"] as IBrush;
                }
                break;
            case 2:
                if (EnvironmentUtil.IsWindow || EnvironmentUtil.IsMac) {
                    window.AcrylicMaterial.IsVisible = true;
                    window.TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                } else {
                    window.Background = Application.Current.Resources["ColourBackground"] as IBrush;
                }
                break;
            case 3:
                window.imageBox.IsVisible = true;
                window.imageBox.Source = _settingService.Data.ImagePath;
                window.imageBox.BlurRadius = _settingService.Data.BlurRadius;
                window.imageBox.IsEnableBlur = _settingService.Data.IsEnableBlur;
                break;
        }
    }

    public void RefreshBackground(MainWindow window) {
        window.imageBox.IsVisible = false;
        window.Background = Brushes.Transparent;
        window.AcrylicMaterial.IsVisible = false;

        switch (_settingService.Data.BackgroundIndex) {
            case 0:
                window.Background = Application.Current.Resources["ColourBackground"] as IBrush;
                break;
            case 1:
                if (EnvironmentUtil.IsWindow) {
                    window.TransparencyLevelHint = [WindowTransparencyLevel.Mica];
                } else {
                    window.Background = Application.Current.Resources["ColourBackground"] as IBrush;
                }
                break;
            case 2:
                if (EnvironmentUtil.IsWindow || EnvironmentUtil.IsMac) {
                    window.AcrylicMaterial.IsVisible = true;
                    window.TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                } else {
                    window.Background = Application.Current.Resources["ColourBackground"] as IBrush;
                }
                break;
            case 3:
                window.imageBox.IsVisible = true;
                window.imageBox.Source = _settingService.Data.ImagePath;
                window.imageBox.BlurRadius = _settingService.Data.BlurRadius;
                window.imageBox.IsEnableBlur = _settingService.Data.IsEnableBlur;
                break;
        }
    }

    //public async void SetBackground(int type) {
    //    if (Design.IsDesignMode) {
    //        return;
    //    }

    //    var main = _mainWindow as MainWindow;
    //    if (main is null) {
    //        return;
    //    }

    //    main.Background = Brushes.Transparent;
    //    main.AcrylicMaterial.IsVisible = false;
    //    main.imageBox.IsVisible = false;

    //    switch (type) {
    //        case 0:
    //            main.TransparencyLevelHint = [WindowTransparencyLevel.Mica];
    //            break;
    //        case 1:
    //            main.AcrylicMaterial.IsVisible = true;
    //            main.TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
    //            var tintColor = main.ActualThemeVariant == ThemeVariant.Dark ? Colors.Black : Colors.White;

    //            main.AcrylicMaterial.Material = new() {
    //                TintOpacity = 0.5d,
    //                TintColor = tintColor,
    //                MaterialOpacity = 0.4d,
    //                BackgroundSource = AcrylicBackgroundSource.Digger
    //            };
    //            break;
    //        case 2:
    //            main.imageBox.IsVisible = true;
    //            string path = _settingService.Data.ImagePath;
    //            if (string.IsNullOrEmpty(path)) {
    //                _dialogService = App.ServiceProvider.GetService<DialogService>();

    //                var result = await Task.Run(async () => await _dialogService.OpenFilePickerAsync([
    //                    new FilePickerFileType("图像文件") { Patterns = new List<string>() { "*.png", "*.jpg", "*.jpeg", "*.tif", "*.tiff" } }
    //                ], "打开文件"));

    //                if (result is null) {
    //                    return;
    //                }

    //                path = result.FullName;
    //            }

    //            var mainVM = (main.DataContext as MainWindowViewModel);
    //            mainVM.ImagePath = path;
    //            mainVM.BlurRadius = _settingService.Data.BlurRadius;
    //            mainVM.IsEnableBlur = _settingService.Data.IsEnableBlur;
    //            _settingService.Data.ImagePath = path;
    //            break;
    //    }
    //}

    public void SetCurrentTheme(int index) {
        Application.Current.RequestedThemeVariant = index switch {
            0 => ThemeVariant.Light,
            1 => ThemeVariant.Dark,
            2 => ThemeVariant.Default,
            _ => ThemeVariant.Default,
        };
    }

    public void ApplyDefaultFont() {
        Application.Current.Resources["DefaultFontFamily"] = $"{DINPro}";
    }
}