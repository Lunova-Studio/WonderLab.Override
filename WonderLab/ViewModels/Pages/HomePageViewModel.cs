using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Utilities;
using System;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ViewModelBase {
    public string Info => $"OS: {EnvironmentUtil.GetPlatformName()}";
}