using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages.Oobe;

public sealed partial class CompletedPageViewModel : PageViewModelBase {
    private readonly SettingService _settingService;

    [ObservableProperty] private string _lottiePath;

    public CompletedPageViewModel(SettingService settingService) {
        _settingService = settingService;
    }

    protected override void OnNavigated() {
        LottiePath = "resm:WonderLab.Assets.Lotties.OOBE_Confetti.json";
    }
}