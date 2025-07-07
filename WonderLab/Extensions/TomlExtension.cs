using Tomlyn.Model;

namespace WonderLab.Extensions;
public static class TomlExtension {
    public static string GetString(this TomlTable table, string key) =>
        !table.ContainsKey(key) ? string.Empty : table[key].ToString();
}