using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WonderLab.Controls.Experimental.BreadcrumbBar;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class SettingNavigationPageViewModel : ObservableObject {
    [ObservableProperty] private bool _isForward;
    [ObservableProperty] private object _activePage;
    [ObservableProperty] private string _activePageKey;
    [ObservableProperty] private bool _isPaginationMode;
    [ObservableProperty] private int _activePageIndex = -1;
    [ObservableProperty] private double _contentBarOpactiy = 1;
    [ObservableProperty] private ObservableCollection<string> _headerItems = ["Settings"];

    public AvaloniaPageProvider PageProvider { get; }

    public SettingNavigationPageViewModel(AvaloniaPageProvider avaloniaPageProvider) {
        PageProvider = avaloniaPageProvider;
    }

    [RelayCommand]
    private void OnLoaded() {
    }

    [RelayCommand]
    private void OnItemClicked(BreadcrumbBarItemClickedEventArgs arg) {
        if (HeaderItems.Count != 2 || arg.Index != 0 || IsPaginationMode)
            return;

        ContentBarOpactiy = 1;
        ActivePageKey = string.Empty;
        IsForward = !IsForward;

        HeaderItems.Remove(HeaderItems.Last());
    }

    [RelayCommand]
    private void ChangeActivePage(object index) {
        if (HeaderItems.Count == 2 && !IsPaginationMode)
            return;
        else if (HeaderItems.Count == 2 && IsPaginationMode)
            HeaderItems.Remove(HeaderItems.Last());

        var intIndex = Convert.ToInt32(index);
        ActivePageKey = intIndex switch {
            0 => "Setting/Launch",
            1 => "Setting/Java",
            2 => "Setting/Account",
            3 => "Setting/Network",
            4 => "Setting/Appearance",
            5 => "Setting/About",
            _ => "Setting/Launch"
        };

        IsForward = !IsForward;
        HeaderItems.Add(ActivePageKey.Split("/")[1]);
        ContentBarOpactiy = IsPaginationMode ? 1 : 0;
    }

    partial void OnIsPaginationModeChanged(bool value) {
        ContentBarOpactiy = 1;

        if (!value && HeaderItems.Count is 2)
            HeaderItems.Remove(HeaderItems.Last());
    }
}