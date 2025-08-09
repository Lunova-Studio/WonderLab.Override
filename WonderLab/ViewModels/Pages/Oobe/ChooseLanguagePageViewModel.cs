using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WonderLab.Classes.Models.I18n;
using WonderLab.Override.I18n;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class ChooseLanguagePageViewModel : PageViewModelBase {
    private readonly SettingService _settingService;

    [ObservableProperty] private LanguageInfo _activeLanguage;

    public static List<LanguageInfo> Languages => [
        new("zh-Hans", "中文（简体）"),
        new("zh-Hant", "中文（繁體）"),
        new("zh-lzh", "中文（文言文）"),
        new("en-US", "English (United States)"),
        new("ja-JP", "日本語 (日本)"),
    ];

    public ChooseLanguagePageViewModel(SettingService settingService) {
        _settingService = settingService;

        ActiveLanguage = _settingService.Setting.LanguageCode is null 
            ? GetDefaultLanguage() 
            : Languages.FirstOrDefault(lang => lang.LanguageCode == _settingService.Setting.LanguageCode);
    }

    public static LanguageInfo GetDefaultLanguage() {

        var culture = CultureInfo.CurrentUICulture;
        var code = culture.Name switch {
            "zh-CN" or "zh-SG" => "zh-Hans",
            "zh-TW" or "zh-HK" or "zh-MO" => "zh-Hant",
            "en-US" => "en-US",
            "ja-JP" => "ja-JP",
            _ => "en-US" // 默认语言
        };

        return Languages.FirstOrDefault(lang => lang.LanguageCode == code)
               ?? Languages.First();
    }

    partial void OnActiveLanguageChanged(LanguageInfo value) {
        I18nExtension.LanguageCode = value.LanguageCode;
        _settingService.Setting.LanguageCode = value.LanguageCode;
    }
}