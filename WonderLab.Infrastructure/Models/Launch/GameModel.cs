using MinecraftLaunch.Classes.Models.Game;

namespace WonderLab.Infrastructure.Models.Launch;

public record GameModel {
    public GameEntry Entry { get; }
    public GameJsonModel Model { set; get; }

    public GameModel(GameEntry entry, GameJsonModel model) {
        Entry = entry;
        Model = model;
    }
}

public record GameJsonModel {
    public bool IsCollection { set; get; }
    public string Id { set; get; }
    public string MinecraftFolder { set; get; }
}