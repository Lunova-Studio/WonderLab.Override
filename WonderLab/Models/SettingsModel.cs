using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WonderLab.Models;

public record SettingsModel {
    [JsonPropertyName("isEnableMirror")] public bool IsEnableMirror { get; set; }
    [JsonPropertyName("isCompletedOOBE")] public bool IsCompletedOOBE { get; set; }

    [JsonPropertyName("maxThread")] public int MaxThread { get; set; } = 128;

    [JsonPropertyName("languageCode")] public string LanguageCode { get; set; } = null;

    [JsonPropertyName("miencraftFolders")] public IEnumerable<MinecraftFolderModel> MiencraftFolders = [];
}

public record MinecraftFolderModel {
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("path")] public string Path { get; set; }

    [JsonIgnore] public string DisplayText => $"{Id} ({Path})";
}

[JsonSerializable(typeof(SettingsModel))]
public sealed partial class SettingModelJsonContext : JsonSerializerContext;