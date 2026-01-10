using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ObservableCollections;
using System.Threading.Tasks;
using WonderLab.Models;
using WonderLab.Services.Game;

namespace WonderLab.ViewModels.Pages;

public sealed partial class MinecraftPageViewModel : ObservableObject {
    [ObservableProperty] private string _activePage;
    [ObservableProperty] private object _activeMinecraft;
    [ObservableProperty] private MinecraftFolderModel _activeMinecraftFolder;

    public INotifyCollectionChangedSynchronizedViewList<FavoritesModel> Minecrafts { get; }
    public INotifyCollectionChangedSynchronizedViewList<MinecraftFolderModel> MinecraftFolders { get; }

    public MinecraftPageViewModel(MinecraftService minecraftService) {
        Minecrafts = minecraftService.Minecrafts.ToNotifyCollectionChanged();
        MinecraftFolders = minecraftService.MinecraftFolders.ToNotifyCollectionChanged();

#if DEBUG
        minecraftService.MinecraftFolders.Add(new() {
            Id = "Temp",
            Path = "D:\\GamePackage\\.minecraft"
        });

        minecraftService.ActiveMinecraftFolder("Temp");
#endif
    }

    async partial void OnActiveMinecraftChanged(object value) {
        if (value is MinecraftModel minecraft) {
            if (ActivePage != "GameSetting/Gallery") {
                ActivePage = "GameSetting/Gallery";
                await Task.Delay(100);
            }

            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<MinecraftModel>(minecraft), "ActiveMinecraftChanged");
        }
    }
}