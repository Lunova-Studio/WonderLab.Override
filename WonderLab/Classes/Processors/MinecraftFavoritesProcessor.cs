using MinecraftLaunch.Base.Interfaces;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Models;

namespace WonderLab.Classes.Processors;

public sealed class MinecraftFavoritesProcessor : IDataProcessor {
    private string _filePath;

    public List<FavoritesModel> Favorites { get; set; } = [];

    Dictionary<string, object> IDataProcessor.Datas { get; set; } = [];// 虽然是我写的接口但这个狗屎属性我自己都不想用 :(

    public void Handle(IEnumerable<MinecraftEntry> minecrafts) {
        if (minecrafts.Any()) {
            _filePath = Path.Combine(minecrafts.First()?.MinecraftFolderPath, "wonderlab_profiles.json");
            if (File.Exists(_filePath)) {
                var launcherProfileJson = File.ReadAllText(_filePath, Encoding.UTF8);
                Favorites = [.. launcherProfileJson.Deserialize(new FavoritesModelContext(
                    JsonSerializerUtil.GetDefaultOptions()).ListFavoritesModel)];
            } else {
                Favorites.Add(new() {
                    Title = "Uncategorized",
                    Favorites = [.. minecrafts.Select(x => new MinecraftModel(x, "Uncategorized"))]
                });
            }
        }
    }

    public Task SaveAsync(CancellationToken cancellationToken = default) {
        var json = Favorites?.Serialize(new FavoritesModelContext(
            JsonSerializerUtil.GetDefaultOptions()).ListFavoritesModel);

        return File.WriteAllTextAsync(_filePath, json, cancellationToken);
    }
}