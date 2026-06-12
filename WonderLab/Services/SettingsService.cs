using System;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using WonderLab.Models;
using WonderLab.Utilities;

namespace WonderLab.Services;

public sealed partial class SettingsService : ObservableObject {
    private readonly ILogger<SettingsService> _logger;
    
    [Settings] public SettingsModel Settings { get; private set; } = new();
    
    public SettingsService(ILogger<SettingsService> logger) {
        _logger = logger;
    }

    public async Task LoadAsync() {
        var directory = PathUtil.GetAppDataDirectory();
        var filePath = Path.Combine(directory, "settings.json");
        
        try {
            Directory.CreateDirectory(directory);
            
            await using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                true);

            var settings = await JsonSerializer.DeserializeAsync<SettingsModel>(stream,
                JsonSerializerUtil.GetDefaultOptions());
            
            Settings = settings;
        } catch (FileNotFoundException) {
            Settings = new SettingsModel();
            await SaveAsync();
            
            _logger.LogWarning("Settings file not found: {FilePath}", filePath);
        } catch (Exception ex) {
            Settings = new SettingsModel();
            _logger.LogError(ex, "Failed to load settings");
        }
    }
    
    public async Task SaveAsync() {
        var directory = PathUtil.GetAppDataDirectory();
        var filePath = Path.Combine(directory, "settings.json");

        try {
            Directory.CreateDirectory(directory);

            await using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(
                stream,
                Settings,
                JsonSerializerUtil.GetDefaultOptions());
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to save settings");
        }
    }
}