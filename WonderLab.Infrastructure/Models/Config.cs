using MinecraftLaunch.Classes.Models.Game;
using System.Text.Json.Serialization;
using WonderLab.Infrastructure.Enums;

namespace WonderLab.Infrastructure.Models;

public record Config {
    public JavaEntry ActiveJava { get; set; }

    public ThemeType ThemeType { get; set; }
    public BackgroundType BackgroundType { get; set; }

    public bool IsDebugMode { get; set; }
    public bool IsFullScreen { get; set; }
    public bool IsAutoSelectJava { get; set; }
    public bool IsGameIndependent { get; set; }
    public bool IsAutoAllocateMemory { get; set; }

    public string ActiveGameId { get; set; }
    public string ActiveLanguage { get; set; }
    public string ActiveImagePath { get; set; }
    public string ActiveAccentColor { get; set; }
    public string ActiveMinecraftFolder { get; set; }

    public List<JavaEntry> Javas { get; set; }
    public List<string> MinecraftFolders { get; set; }
}