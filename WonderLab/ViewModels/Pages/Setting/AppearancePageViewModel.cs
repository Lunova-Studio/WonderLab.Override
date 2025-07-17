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
using WonderLab.Classes.Models;
using WonderLab.Classes.Models.I18n;
using WonderLab.Override.I18n;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AppearancePageViewModel : ObservableObject {
    private readonly ThemeService _themeService;
    private readonly SettingService _settingService;
    private readonly ILogger<AppearancePageViewModel> _logger;

    [ObservableProperty] private string _imagePath;
    [ObservableProperty] private int _activeThemeIndex;
    [ObservableProperty] private bool _isEnableSystemColor;
    [ObservableProperty] private bool _isEnableBitmapColor;
    [ObservableProperty] private ColorInfo _activeColor;
    [ObservableProperty] private Variant _activeColorVariant;
    [ObservableProperty] private LanguageInfo _activeLanguage;
    [ObservableProperty] private IReadOnlyList<ColorInfo> _colors;
    [ObservableProperty] private ReadOnlyObservableCollection<BackgroundType> _backgrounds;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBitmapBackground))]
    private BackgroundType _activeBackground;

    public bool IsBitmapBackground => ActiveBackground is BackgroundType.Bitmap;
    public static List<LanguageInfo> Languages => [
        new("zh-Hans", "简体中文"),
        new("zh-Hant", "繁體中文"),
        new("zh-lzh", "文言文"),
        new("en-US", "English"),
        new("ja-JP", "日本語"),
    ];

    public AppearancePageViewModel(SettingService settingService, ThemeService themeService, ILogger<AppearancePageViewModel> logger) {
        _logger = logger;
        _themeService = themeService;
        _settingService = settingService;

        Backgrounds = new(_themeService.BackgroundTypes);

        Colors = _themeService.ColorInfos;
        ImagePath = _settingService.Setting.ImagePath;
        ActiveBackground = _settingService.Setting.ActiveBackground;
        ActiveColorVariant = _settingService.Setting.ActiveColorVariant;
        IsEnableSystemColor = _settingService.Setting.IsEnableSystemColor;
        IsEnableBitmapColor = _settingService.Setting.IsEnableBitmapColor;
        ActiveLanguage = Languages.FirstOrDefault(x => x.LanguageCode == _settingService.Setting.LanguageCode);
        ActiveThemeIndex = _settingService.Setting.ActiveTheme switch {
            ThemeType.Auto => 0,
            ThemeType.Dark => 2,
            ThemeType.Light => 1,
            _ => 0,
        };

        ActiveColor = _themeService.ColorInfos.FirstOrDefault(x => x.ColorData == _settingService.Setting.ActiveColor) ??
            _themeService.ColorInfos.FirstOrDefault(x => x.Key == "Red")!;
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
                case nameof(ActiveThemeIndex):
                    _settingService.Setting.ActiveTheme = ActiveThemeIndex switch {
                        0 => ThemeType.Auto,
                        1 => ThemeType.Light,
                        2 => ThemeType.Dark,
                        _ => ThemeType.Auto,
                    };

                    Application.Current.RequestedThemeVariant = ActiveThemeIndex switch {
                        0 => ThemeVariant.Default,
                        2 => ThemeVariant.Dark,
                        1 => ThemeVariant.Light,
                        _ => ThemeVariant.Light
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
                        _themeService.UpdateColorScheme(ActiveColorVariant, ActiveColor.ColorData);
                    break;

                case nameof(IsEnableSystemColor):
                    IsEnableBitmapColor = _settingService.Setting.IsEnableBitmapColor = !IsEnableSystemColor && IsEnableBitmapColor;
                    _settingService.Setting.IsEnableSystemColor = IsEnableSystemColor;
                    if (IsEnableSystemColor)
                        _themeService.UseSystemColor();
                    else
                        _themeService.UpdateColorScheme(ActiveColorVariant, _settingService.Setting.ActiveColor);
                    break;

                case nameof(ActiveColor):
                    _themeService.UpdateColorScheme(ActiveColorVariant, _settingService.Setting.ActiveColor = ActiveColor.ColorData);
                    break;

                case nameof(IsEnableBitmapColor):
                    _settingService.Setting.IsEnableBitmapColor = IsEnableBitmapColor;
                    if (IsEnableBitmapColor)
                        AutoSelectBitmapPrimaryColorAsync();
                    break;

                case nameof(ActiveLanguage):
                    _settingService.Setting.LanguageCode = I18nExtension.LanguageCode = ActiveLanguage.LanguageCode;
                    break;
            }
        }, DispatcherPriority.Background);
    }
}