using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Monet.Shared.Enums;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class ChooseThemePageViewModel : PageViewModelBase {
    private readonly ThemeService _themeService;
    private readonly SettingService _settingService;

    [ObservableProperty] private ThemeType _activeTheme;
    [ObservableProperty] private Variant _activeColorVariant;

    public ChooseThemePageViewModel(SettingService settingService, ThemeService themeService) {
        _themeService = themeService;
        _settingService = settingService;

        ActiveTheme = _settingService.Setting.ActiveTheme;
        ActiveColorVariant = _settingService.Setting.ActiveColorVariant;

        WeakReferenceMessenger.Default.Send(new EnabledChangedMessage(true));
    }

    partial void OnActiveThemeChanged(ThemeType value) {
        _themeService.UpdateThemeVariant(_settingService.Setting.ActiveTheme = value);
    }

    partial void OnActiveColorVariantChanged(Variant value) {
        _themeService.UpdateColorScheme(_settingService.Setting.ActiveColorVariant = value);
    }
}