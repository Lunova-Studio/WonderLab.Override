using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Models.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace WonderLab.Models;

// 沟槽的 TreeView 层级显示不能用字典，所以只能用列表了
public interface ITreeViewNode {
    string Title { get; }
    bool IsHasIcon { get; }
}

public record FavoritesModel : ITreeViewNode {
    [JsonIgnore] public bool IsHasIcon => false;

    [JsonPropertyName("title")] public string Title { get; set; }
    [JsonPropertyName("favorites")] public ObservableCollection<MinecraftModel> Favorites { get; set; }
}

public record MinecraftModel : ITreeViewNode {
    [JsonIgnore] public bool IsHasIcon => true;
    [JsonIgnore] public string Title => Minecraft.Id;

    [JsonPropertyName("favoritesId")] public string FavoritesId { get; set; }
    [JsonPropertyName("minecraft")] public MinecraftEntry Minecraft { get; set; }
    [JsonPropertyName("setting")] public MinecraftSettingModel Setting { get; set; }
    [JsonPropertyName("sessions")] public IEnumerable<PlaySession> Sessions { get; set; } = []; // 所有游戏会话时长记录

    public MinecraftModel() {}

    public MinecraftModel(MinecraftEntry minecraft, string favoritesId) {
        FavoritesId = favoritesId;
        Minecraft = minecraft;
        Setting = new();
    }
}

public record PlaySession {
    [JsonIgnore] public TimeSpan Duration => EndTime - StartTime;

    [JsonPropertyName("endTime")] public DateTime EndTime { get; set; }
    [JsonPropertyName("startTime")] public DateTime StartTime { get; set; }
}

public record MinecraftSettingModel {
    [JsonPropertyName("width")] public int Width { get; set; } = 854;
    [JsonPropertyName("height")] public int Height { get; set; } = 480;

    [JsonPropertyName("isFullScreen")] public bool IsFullScreen { get; set; }
    [JsonPropertyName("isEnableIndependency")] public bool IsEnableIndependency { get; set; } = true;
    [JsonPropertyName("isEnableSpecificSetting")] public bool IsEnableSpecificSetting { get; set; }

    [JsonPropertyName("jvmArgument")] public string JvmArgument { get; set; } = string.Empty;
    [JsonPropertyName("serverAddress")] public string ServerAddress { get; set; } = string.Empty;

    [JsonPropertyName("activeAccount")] public Account ActiveAccount { get; set; }
}

[JsonSerializable(typeof(List<FavoritesModel>))]
public sealed partial class FavoritesModelContext : JsonSerializerContext;