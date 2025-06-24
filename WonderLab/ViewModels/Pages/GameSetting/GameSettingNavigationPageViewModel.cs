using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
using System;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.Launch;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class GameSettingNavigationPageViewModel : DynamicPageViewModelBase {
    private readonly GameService _gameService;
    private readonly MinecraftEntry _minecraftEntry;

    [ObservableProperty] private string _pageKey;
    [ObservableProperty] private int _activePageIndex;

    public string MinecraftId => _minecraftEntry.Id;
    public AvaloniaPageProvider PageProvider { get; }

    public GameSettingNavigationPageViewModel(GameService gameService, AvaloniaPageProvider avaloniaPageProvider) {
        _gameService = gameService;
        _minecraftEntry = _gameService.ActiveGameCache;

        PageProvider = avaloniaPageProvider;
    }

    [RelayCommand]
    private async Task OnLoaded() {
        await Task.Delay(200);
        PageKey = "GameSetting/Setting";
    }

    [RelayCommand]
    private async Task Save() {
        try {
            await MinecraftParser.DataProcessors.FirstOrDefault()
                .SaveAsync();

            WeakReferenceMessenger.Default.Send(new NotificationMessage("保存成功", NotificationType.Success));
        } catch (Exception) { }
    }

    public override async void Close() {
        base.Close();
        await Save();
    }

    partial void OnActivePageIndexChanged(int value) {
        PageKey = value switch {
            0 => "GameSetting/Setting",
            1 => "GameSetting/Resourcepack",
            2 => "GameSetting/Mod",
            3 => "GameSetting/Shaderpack",
            _ => PageKey ?? "GameSetting/Setting",
        };
    }
}