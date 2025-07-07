using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Monet.Avalonia.Extensions;
using Monet.Shared.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AppearancePageViewModel : ObservableObject {
    private readonly ThemeService _themeService;
    private readonly SettingService _settingService;
    private readonly ILogger<AppearancePageViewModel> _logger;

    [ObservableProperty] private string _imagePath;
    [ObservableProperty] private ThemeType _activeTheme;
    [ObservableProperty] private bool _isEnableSystemColor;
    [ObservableProperty] private bool _isEnableBitmapColor;
    [ObservableProperty] private Variant _activeColorVariant;
    [ObservableProperty] private KeyValuePair<uint, string> _activeColor;
    [ObservableProperty] private IReadOnlyDictionary<uint, string> _colors;
    [ObservableProperty] private ReadOnlyObservableCollection<BackgroundType> _backgrounds;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBitmapBackground))]
    private BackgroundType _activeBackground;

    public bool IsBitmapBackground => ActiveBackground is BackgroundType.Bitmap;

    public AppearancePageViewModel(SettingService settingService, ThemeService themeService, ILogger<AppearancePageViewModel> logger) {
        _logger = logger;
        _themeService = themeService;
        _settingService = settingService;

        Backgrounds = new(_themeService.BackgroundTypes);

        Colors = _themeService.MonetColors;
        ImagePath = _settingService.Setting.ImagePath;
        ActiveTheme = _settingService.Setting.ActiveTheme;
        ActiveBackground = _settingService.Setting.ActiveBackground;
        ActiveColorVariant = _settingService.Setting.ActiveColorVariant;
        IsEnableSystemColor = _settingService.Setting.IsEnableSystemColor;
        IsEnableBitmapColor = _settingService.Setting.IsEnableBitmapColor;
        ActiveColor = _themeService.MonetColors.FirstOrDefault(x => x.Key == _settingService.Setting.ActiveColor);
    }

    [RelayCommand]
    private void OnLoaded() {
        PropertyChanged += OnPropertyChanged;
    }

    [RelayCommand]
    private Task BrowserImage() => Dispatcher.UIThread.InvokeAsync(async () => {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            var result = await lifetime.MainWindow.StorageProvider.OpenFilePickerAsync(new() {
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (result is { Count: > 0 })
                ImagePath = result[0].Path.LocalPath;
        }
    });

    private Task AutoSelectBitmapPrimaryColorAsync() => Task.Run(async () => {
        var image = Image.Load(ImagePath).CloneAs<Rgba32>();
        if (image is not null) {
            var color = image.QuantizeAndGetPrimaryColors()
                .Select(x => x.ToUInt32())
                .FirstOrDefault();

            await Dispatcher.UIThread.InvokeAsync(() => {
                _themeService.UpdateColorScheme(ActiveColorVariant,
                    _settingService.Setting.ActiveColor = color);
            });
        }
    });

    private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        await Dispatcher.UIThread.InvokeAsync(() => {
            switch (e.PropertyName) {
                case nameof(ActiveTheme):
                    _settingService.Setting.ActiveTheme = ActiveTheme;
                    Application.Current.RequestedThemeVariant = ActiveTheme switch {
                        ThemeType.Auto => ThemeVariant.Default,
                        ThemeType.Dark => ThemeVariant.Dark,
                        ThemeType.Light => ThemeVariant.Light,
                        _ => ThemeVariant.Light,
                    };
                    break;

                case nameof(ImagePath):
                    if (!File.Exists(ImagePath))
                        return;
                    _themeService.UpdateBackgroundType(ActiveBackground, _settingService.Setting.ImagePath = ImagePath);
                    if (IsEnableBitmapColor && IsBitmapBackground)
                        AutoSelectBitmapPrimaryColorAsync();
                    break;

                case nameof(ActiveBackground):
                    _themeService.UpdateBackgroundType(_settingService.Setting.ActiveBackground = ActiveBackground, ImagePath);
                    if (IsEnableBitmapColor && IsBitmapBackground)
                        AutoSelectBitmapPrimaryColorAsync();
                    else if (!IsBitmapBackground)
                        IsEnableBitmapColor = false;
                    break;

                case nameof(ActiveColorVariant):
                    _settingService.Setting.ActiveColorVariant = ActiveColorVariant;
                    if (IsEnableSystemColor)
                        _themeService.UseSystemColor();
                    else
                        _themeService.UpdateColorScheme(ActiveColorVariant, ActiveColor.Key);
                    break;

                case nameof(IsEnableSystemColor):
                    // 切换系统色时，自动关闭图片主色提取
                    IsEnableBitmapColor = _settingService.Setting.IsEnableBitmapColor = !IsEnableSystemColor && IsEnableBitmapColor;
                    _settingService.Setting.IsEnableSystemColor = IsEnableSystemColor;
                    if (IsEnableSystemColor)
                        _themeService.UseSystemColor();
                    else
                        _themeService.UpdateColorScheme(ActiveColorVariant, _settingService.Setting.ActiveColor);
                    break;

                case nameof(ActiveColor):
                    _themeService.UpdateColorScheme(ActiveColorVariant, _settingService.Setting.ActiveColor = ActiveColor.Key);
                    break;

                case nameof(IsEnableBitmapColor):
                    _settingService.Setting.IsEnableBitmapColor = IsEnableBitmapColor;
                    if (IsEnableBitmapColor)
                        AutoSelectBitmapPrimaryColorAsync();
                    break;
            }
        }, DispatcherPriority.Background);
    }
}