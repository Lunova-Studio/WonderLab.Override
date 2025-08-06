using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
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

    [ObservableProperty] private int _pageIndex = 1;
    [ObservableProperty] private bool _isNextButtonEnabled = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBackButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsSkipButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsNextButtonVisible))]
    private string _activePageKey;

    public AvaloniaPageProvider PageProvider { get; }

    public bool IsSkipButtonVisible => ActivePageKey is OOBE_QuickImport;
    public bool IsBackButtonVisible => ActivePageKey is not OOBE_ChooseLanguage;
    public bool IsNextButtonVisible => ActivePageKey is not OOBE_Completed && ActivePageKey is not OOBE_QuickImport;

    public OobeWindowViewModel(AvaloniaPageProvider pageProvider) {
        PageProvider = pageProvider;

        WeakReferenceMessenger.Default.Register<EnabledChangedMessage>(this, (_, args) => {
            IsNextButtonEnabled = args.IsEnabled;
        });

        WeakReferenceMessenger.Default.Register<PageNotificationMessage>(this, (_, arg) => {
            ActivePageKey = arg.PageKey;
        });
    }

    [RelayCommand]
    private void OnLoaded() {
        ActivePageKey = OOBE_ChooseLanguage;
    }

    [RelayCommand]
    private void Next() {
        if (ActivePageKey is OOBE_ChooseLanguage) {
            PageIndex++;
            ActivePageKey = OOBE_ChooseTheme;
        } else if (ActivePageKey is OOBE_ChooseTheme) {
            PageIndex++;
            ActivePageKey = OOBE_QuickImport;
        } else if (ActivePageKey is OOBE_QuickImport) {
            PageIndex++;
            ActivePageKey = OOBE_ChooseMinecraft;
        } else if (ActivePageKey is OOBE_ChooseMinecraft) {
            PageIndex++;
            ActivePageKey = OOBE_ChooseJava;
        } else if (ActivePageKey is OOBE_ChooseJava) {
            PageIndex++;
            ActivePageKey = OOBE_AddAccount;
        } else if (ActivePageKey is OOBE_AddAccount) {
            PageIndex++;
            ActivePageKey = OOBE_Completed;
        }

        IsNextButtonEnabled = false;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task Back() => await Dispatcher.UIThread.InvokeAsync(() => {
        if (PageIndex - 1 < 1 && ActivePageKey != OOBE_ChooseLanguage)
            return;

        if (ActivePageKey is OOBE_ChooseTheme)
            ActivePageKey = OOBE_ChooseLanguage;
        else if (ActivePageKey is OOBE_QuickImport)
            ActivePageKey = OOBE_ChooseTheme;
        else if (ActivePageKey is OOBE_ChooseMinecraft)
            ActivePageKey = OOBE_QuickImport;
        else if (ActivePageKey is OOBE_ChooseJava)
            ActivePageKey = OOBE_ChooseMinecraft;
        else if (ActivePageKey is OOBE_AddAccount)
            ActivePageKey = OOBE_ChooseJava;
        else if (ActivePageKey is OOBE_Completed)
            ActivePageKey = OOBE_AddAccount;

        PageIndex--;
        IsNextButtonEnabled = true;
    });
}