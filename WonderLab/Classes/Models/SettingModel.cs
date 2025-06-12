using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Models.Game;
using Monet.Shared.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WonderLab.Classes.Enums;

namespace WonderLab.Classes.Models;

public record SettingModel {
    [JsonPropertyName("isFullScreen")] public bool IsFullScreen { get; set; }
    [JsonPropertyName("isEnableMirror")] public bool IsEnableMirror { get; set; }
    [JsonPropertyName("isAutoSelectJava")] public bool IsAutoSelectJava { get; set; }
    [JsonPropertyName("isEnableSystemColor")] public bool IsEnableSystemColor { get; set; } = true;
    [JsonPropertyName("isEnableIndependency")] public bool IsEnableIndependency { get; set; } = true;

    [JsonPropertyName("width")] public int Width { get; set; } = 854;
    [JsonPropertyName("height")] public int Height { get; set; } = 480;
    [JsonPropertyName("maxThread")] public int MaxThread { get; set; } = 128;
    [JsonPropertyName("activeColor")] public uint ActiveColor { get; set; }
    [JsonPropertyName("maxMemorySize")] public int MaxMemorySize { get; set; }
    [JsonPropertyName("minMemorySize")] public int MinMemorySize { get; set; } = 512;

    [JsonPropertyName("imagePath")] public string ImagePath { get; set; } = string.Empty;
    [JsonPropertyName("activeGameId")] public string ActiveGameId { get; set; } = string.Empty;
    [JsonPropertyName("serverAddress")] public string ServerAddress { get; set; } = string.Empty;
    [JsonPropertyName("activeMinecraftFolder")] public string ActiveMinecraftFolder { get; set; } = string.Empty;

    [JsonPropertyName("activeJava")] public JavaEntry ActiveJava { get; set; }
    [JsonPropertyName("activeAccount")] public Account ActiveAccount { get; set; }

    [JsonPropertyName("activeTheme")] public ThemeType ActiveTheme { get; set; } = ThemeType.Auto;
    [JsonPropertyName("activeColorVariant")] public Variant ActiveColorVariant { get; set; } = Variant.Default;
    [JsonPropertyName("activeBackgroundType")] public BackgroundType ActiveBackground { get; set; } = BackgroundType.SolidColor;

    [JsonPropertyName("accounts")] public List<Account> Accounts { get; set; } = [];
    [JsonPropertyName("javaEntrys")] public List<JavaEntry> Javas { get; set; } = [];
    [JsonPropertyName("minecraftFolders")] public List<string> MinecraftFolders { get; set; } = [];
    [JsonPropertyName("imageColorPalette")] public List<uint> ImageColorPalette { get; set; } = [];
}

[JsonSerializable(typeof(SettingModel))]
public sealed partial class SettingModelJsonContext : JsonSerializerContext;