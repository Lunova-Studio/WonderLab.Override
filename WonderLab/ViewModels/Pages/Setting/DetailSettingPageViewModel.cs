using Avalonia.Controls.Notifications;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.ComponentModel;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Services;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Dialogs.Setting;
using WonderLab.Views.Windows;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class DetailSettingPageViewModel : ViewModelBase {
    private readonly ThemeService _themeService;
    private readonly DialogService _dialogService;
    private readonly WindowService _windowService;
    private readonly SettingService _settingService;
    private readonly LanguageService _languageService;
    private readonly NotificationService _notificationService;
    private readonly WeakReferenceMessenger _weakReferenceMessenger;

    [ObservableProperty] private bool _isDebugMode = false;
    [ObservableProperty] private bool _isEnableBlur = false;
    [ObservableProperty] private bool _isAlignCenter = false;

    [ObservableProperty] private int _blurRadius = 0;
    [ObservableProperty] private int _themeIndex = 0;
    [ObservableProperty] private int _parallaxMode = 0;
    [ObservableProperty] private int _languageIndex = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsImageMode))]
    private int _backgroundIndex = 0;

    public bool IsImageMode => BackgroundIndex is 3;

    public List<FontFamily> Fonts { get; }

    public DetailSettingPageViewModel(
        ThemeService themeService, 
        WindowService windowService,
        DialogService dialogService,
        SettingService settingService, 
        LanguageService languageService,
        NotificationService notificationService,
        WeakReferenceMessenger weakReferenceMessenger) {
        _themeService = themeService;
        _windowService = windowService;
        _settingService = settingService;
        _languageService = languageService;
        _notificationService = notificationService;
        _dialogService = dialogService;
        _weakReferenceMessenger = weakReferenceMessenger;

        BlurRadius = _settingService.Data.BlurRadius;
        ThemeIndex = _settingService.Data.ThemeIndex;
        IsDebugMode = _settingService.Data.IsDebugMode;
        ParallaxMode = _settingService.Data.ParallaxMode;
        IsEnableBlur = _settingService.Data.IsEnableBlur;
        IsAlignCenter = _settingService.Data.IsAlignCenter;
        LanguageIndex = _settingService.Data.LanguageIndex;
        BackgroundIndex = _settingService.Data.BackgroundIndex;

        Fonts = [.. FontManager.Current.SystemFonts];
    }

    [RelayCommand]
    private void Notification(string text) {
        _notificationService.QueueJob(new NotificationViewData {
            Title = "Info",
            Content = text,
            NotificationType = NotificationType.Information
        });
    }

    [RelayCommand]
    private void Search() {
        _settingService.Data.ImagePath = string.Empty;
        _themeService.RefreshBackground(_windowService.MainWindow as MainWindow);
    }

    [RelayCommand]
    private void PressOobe() {
        _dialogService.ShowContentDialog<RecheckToOobeDialogViewModel>();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        switch (e.PropertyName) {
            case nameof(ThemeIndex):
                _themeService.SetCurrentTheme(ThemeIndex);
                _settingService.Data.ThemeIndex = ThemeIndex;
                break;
            case nameof(BackgroundIndex):
                _settingService.Data.BackgroundIndex = BackgroundIndex;
                _themeService.RefreshBackground(_windowService.MainWindow as MainWindow);
                break;
            case nameof(LanguageIndex):
                _languageService.SetLanguage(LanguageIndex);
                _settingService.Data.LanguageIndex = LanguageIndex;
                break;
            case nameof(IsDebugMode):
                _settingService.Data.IsDebugMode = IsDebugMode;
                break;
            case nameof(IsAlignCenter):
                _settingService.Data.IsAlignCenter = IsAlignCenter;
                _weakReferenceMessenger.Send(new AlignCenterChangeMessage(IsAlignCenter));
                break;
            case nameof(IsEnableBlur):
                _settingService.Data.IsEnableBlur = IsEnableBlur;
                _weakReferenceMessenger.Send(new BlurEnableMessage(IsEnableBlur));
                break;
            case nameof(ParallaxMode):
                _settingService.Data.ParallaxMode = ParallaxMode;
                _weakReferenceMessenger.Send(new ParallaxModeChangeMessage(ParallaxMode));
                break;
            case nameof(BlurRadius):
                _settingService.Data.BlurRadius = BlurRadius;
                _weakReferenceMessenger.Send(new BlurRadiusChangeMessage(BlurRadius));
                break;
        }
    }
}