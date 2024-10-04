using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Linq;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class MinecraftListPageViewModel : ViewModelBase {
    [ObservableProperty] private object _activeMinecraft;
    [ObservableProperty] private IEnumerable _minecraftList;

    public MinecraftListPageViewModel(DownloadService downloadService) {
        RunBackgroundWork(async () => {
            MinecraftList = (await downloadService.GetMinecraftListAsync()).Select(x => new MinecraftViewData(x));
        });
    }
}