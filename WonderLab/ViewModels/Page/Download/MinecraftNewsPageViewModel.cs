using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace WonderLab.ViewModels.Page.Download;

public sealed partial class MinecraftNewsPageViewModel : ObservableObject {
    [ObservableProperty] private bool _isNetworkAvailable;

    public List<string> MinecraftNews => [
        "Minecraft 1.21.4 Release Candidate 1",
        "Minecraft 1.21.4 Release Candidate 2",
        "Minecraft 1.21.4 Release Candidate 3",
    ];

    public MinecraftNewsPageViewModel() {
    }

    [RelayCommand]
    private void OnLoaded() {
        IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
    }

    [RelayCommand]
    private void RefreshNetwork() {
        IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
    }
}