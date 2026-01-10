using MinecraftLaunch.Components.Parser;
using ObservableCollections;
using System;
using System.IO;
using System.Linq;
using WonderLab.Classes.Processors;
using WonderLab.Models;

namespace WonderLab.Services.Game;

public sealed class MinecraftService {
    public ObservableList<FavoritesModel> Minecrafts { get; } = [];
    public ObservableList<MinecraftFolderModel> MinecraftFolders { get; } = [];

    public event Action MinecraftFolderChanged;

    public MinecraftParser Parser { get; private set; }

    public MinecraftService(SettingService settingService) {
        MinecraftFolders.AddRange(settingService.Setting.MiencraftFolders);
    }

    public void ActiveMinecraftFolder(string key) {
        var path = MinecraftFolders.FirstOrDefault(x => x.Id == key)?.Path;
        if (Directory.Exists(path))
            Parser = path;
        else
            throw new DirectoryNotFoundException();

        MinecraftFolderChanged?.Invoke();
        Refresh();
    }

    public void Refresh() {
        _ = Parser.GetMinecrafts();

        Minecrafts.Clear();
        if (MinecraftParser.DataProcessors.TryGetValue("Favorites", out var processor)
            && processor is MinecraftFavoritesProcessor favoritesProcessor) {
            Minecrafts.AddRange(favoritesProcessor.Favorites);
        }
    }
}
