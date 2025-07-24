using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;

namespace WonderLab.ViewModels.Windows;

public sealed partial class OobeWindowViewModel : ObservableObject {
    private readonly IEnumerable<ISettingImporter> _settingImporters;

    public OobeWindowViewModel(IEnumerable<ISettingImporter> settingImporters) {
        _settingImporters = settingImporters;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        var hmclSettingImporter = _settingImporters
            .First(x => x.Type == "HMCL");

        hmclSettingImporter.LauncherPath = @"C:\Users\wxysd\Desktop\总整包\MC\mc启动器\HMCL\HMCL.exe";
        await hmclSettingImporter.ImportAsync();
    });
}