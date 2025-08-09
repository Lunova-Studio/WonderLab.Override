using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.ViewModels.Windows;

public sealed partial class OobeWindowViewModel : ObservableObject {
    private const string OOBE_Completed = "OOBE/Completed";
    private const string OOBE_AddAccount = "OOBE/AddAccount";
    private const string OOBE_ChooseJava = "OOBE/ChooseJava";
    private const string OOBE_QuickImport = "OOBE/QuickImport";
    private const string OOBE_ChooseTheme = "OOBE/ChooseTheme";
    private const string OOBE_ChooseLanguage = "OOBE/ChooseLanguage";
    private const string OOBE_ChooseMinecraft = "OOBE/ChooseMinecraft";

    private int _pageIndex;

    [ObservableProperty] private bool _isNextButtonEnabled = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PageIndex))]
    [NotifyPropertyChangedFor(nameof(PageLanguageKey))]
    [NotifyPropertyChangedFor(nameof(IsBackButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsSkipButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsNextButtonVisible))]
    private string _activePageKey;

    public AvaloniaPageProvider PageProvider { get; }

    public int PageIndex => _pageIndex + 1;
    public string PageLanguageKey => ActivePageKey?.Replace('/', '_');
    public bool IsSkipButtonVisible => ActivePageKey is OOBE_QuickImport;
    public bool IsBackButtonVisible => ActivePageKey is not OOBE_ChooseLanguage;
    public bool IsNextButtonVisible => ActivePageKey is not OOBE_Completed && ActivePageKey is not OOBE_QuickImport;

    public List<string> OOBE_Pages { get; } = [
        OOBE_ChooseLanguage,
        OOBE_ChooseTheme,
        OOBE_QuickImport,
        OOBE_ChooseMinecraft,
        OOBE_ChooseJava,
        OOBE_AddAccount,
        OOBE_Completed
    ];

    public OobeWindowViewModel(AvaloniaPageProvider pageProvider) {
        PageProvider = pageProvider;

        WeakReferenceMessenger.Default.Register<EnabledChangedMessage>(this, (_, args) => {
            IsNextButtonEnabled = args.IsEnabled;
        });

        WeakReferenceMessenger.Default.Register<PageNotificationMessage>(this, (_, arg) => {
            ActivePageKey = arg.PageKey;
            _pageIndex++;
        });
    }

    [RelayCommand]
    private void OnLoaded() {
        ActivePageKey = OOBE_ChooseLanguage;
    }

    [RelayCommand]
    private void Next() {
        ChangePage(_pageIndex + 1, false);
    }

    [RelayCommand]
    private void Back() {
        ChangePage(_pageIndex - 1, true);
    }

    private void ChangePage(int newIndex, bool? setNextEnabled = null) {
        if (newIndex < 0 || newIndex >= OOBE_Pages.Count)
            return;

        _pageIndex = newIndex;
        ActivePageKey = OOBE_Pages[newIndex];

        if (setNextEnabled.HasValue)
            IsNextButtonEnabled = setNextEnabled.Value;
    }
}