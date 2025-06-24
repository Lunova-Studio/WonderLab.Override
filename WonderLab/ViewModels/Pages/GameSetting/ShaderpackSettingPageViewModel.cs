using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Services.Auxiliary;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class ShaderpackSettingPageViewModel : ObservableObject {
    private readonly ShaderpackService _shaderpackService;
    private readonly ILogger<ShaderpackSettingPageViewModel> _logger;

    [ObservableProperty] private bool _isIris;
    [ObservableProperty] private bool _isEnableShader;
    [ObservableProperty] private bool _hasShaderpacks;
    [ObservableProperty] private Shaderpack _activeShaderpack;
    [ObservableProperty] private ReadOnlyObservableCollection<Shaderpack> _shaderpacks;

    public ShaderpackSettingPageViewModel(ShaderpackService shaderpackService, ILogger<ShaderpackSettingPageViewModel> logger) {
        _shaderpackService = shaderpackService;
        _logger = logger;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        await RefreshCommand.ExecuteAsync(default);

        IsIris = _shaderpackService.IsIris;
        IsEnableShader = _shaderpackService.IsEnableShaders;

        PropertyChanged += OnPropertyChanged;
    });

    [RelayCommand]
    private Task Refresh() => Task.Run(async () => {
        _shaderpackService.Init();
        await _shaderpackService.LoadAllAsync(default);
        Shaderpacks = new(_shaderpackService.Shaderpacks);
        ActiveShaderpack = Shaderpacks.FirstOrDefault(x => x.IsEnabled);

        HasShaderpacks = Shaderpacks.Count > 0;
        _logger.LogInformation("Loaded {count} shaderpack", Shaderpacks.Count);
    });

    private Task SaveAsync() => Task.Run(async () => {
        await _shaderpackService.SaveToConfigAsync(default);
    });

    private Task ChangeEnableStatusAsync() => Task.Run(async () => {
        _shaderpackService.ChangeEnableStatus(IsEnableShader);
        await SaveAsync();
    });

    private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(IsEnableShader):
                await ChangeEnableStatusAsync();
                break;
            case nameof(ActiveShaderpack) when ActiveShaderpack is not null:
                foreach (var shaderpack in _shaderpackService.Shaderpacks)
                    shaderpack.IsEnabled = false;

                ActiveShaderpack.IsEnabled = true;
                await SaveAsync();
                break;
        }
    }
}