using System.Text.Json.Serialization;

namespace WonderLab.Models;

public record SettingsModel {
    // Game Settings
    [JsonPropertyName("isFullscreen")] public bool IsFullscreen { get; set; }
    [JsonPropertyName("isolationVersion")] public bool IsolationVersion { get; set; }
    
    // Appearance Settings 
    [JsonPropertyName("color")] public int Color { get; set; }
    [JsonPropertyName("isFollowSystem")] public bool IsFollowSystem { get; set; }
    
    // Network Settings
    [JsonPropertyName("maxThread")] public int MaxThread { get; set; }
    [JsonPropertyName("isEnableMirror")] public bool IsEnableMirrorDownloadSource { get; set; }
}