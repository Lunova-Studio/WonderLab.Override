using MinecraftLaunch.Classes.Enums;

namespace WonderLab.Classes.Datas;

public sealed class ModLoaderData {
    public string Id { get; }
    public object Entry { get; }
    public LoaderType LoaderType { get; }

    public ModLoaderData(string id, object entry, LoaderType loaderType) {
        Id = id;
        Entry = entry;
        LoaderType = loaderType;
    }
}