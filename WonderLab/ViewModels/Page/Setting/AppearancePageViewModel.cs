using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.Infrastructure.Enums;
using WonderLab.Infrastructure.Models;
using WonderLab.Services;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Page.Setting;

public sealed partial class AppearancePageViewModel : ObservableObject {
    private readonly ThemeService _themeService;
    private readonly ConfigService _configService;

    [ObservableProperty] private bool _isDebugMode;
    [ObservableProperty] private Color _color;
    [ObservableProperty] private ThemeType _themeType;
    [ObservableProperty] private CultureInfo _language;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageColors))]
    private string _activeImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsImageBackground))]
    private BackgroundType _backgroundType;

    public Config Config => _configService.Entries;

    public bool IsImageBackground => BackgroundType is BackgroundType.Image;

    public IEnumerable<Color> ImageColors => ActiveImage?.ToRgba32Bitmap()?.GetPaletteFromBitmap().Select(x => x.Color);

    public List<Color> Colors { get; } = ColorExtension.Colors;

    public List<CultureInfo> Languages { get; } = [
        new("zh-Hans"),
        new("zh-Hant"),
        new("en"),
    ];

    public AppearancePageViewModel(ConfigService configService, ThemeService themeService) {
        _themeService = themeService;
        _configService = configService;
    }

    [RelayCommand]
    private void OnLoaded() {
        _ = Dispatcher.UIThread.InvokeAsync(() => {
            ActiveImage = Config.ActiveImagePath;
            Color = Color.Parse(Config.ActiveAccentColor);
            ThemeType = Config.ThemeType;
            Language = new(Config.ActiveLanguage);
            BackgroundType = Config.BackgroundType;
        });
    }

    [RelayCommand]
    private Task BrowserImage() => Task.Run(async () => {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
            var result = await lifetime.MainWindow.StorageProvider.OpenFilePickerAsync(new() {
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (result is { Count: 0 }) {
                return;
            }

            ActiveImage = result[0].Path.LocalPath;
        }
    });

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case nameof(ActiveImage):
                Config.ActiveImagePath = ActiveImage;
                _themeService.ApplyWindowEffect(BackgroundType);
                break;
            case nameof(Color):
                Config.ActiveAccentColor = Color.ToString();
                _themeService.ApplyAccentColor(Color);
                break;
            case nameof(ThemeType):
                _themeService.ApplyTheme(ThemeType);
                Config.ThemeType = ThemeType;
                break;
            case nameof(Language):
                I18NExtension.Culture = Language;
                Config.ActiveLanguage = Language.Name;
                break;
            case nameof(BackgroundType):
                _themeService.ApplyWindowEffect(BackgroundType);
                Config.BackgroundType = BackgroundType;
                break;
        }
    }
}