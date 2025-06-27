using MinecraftLaunch.Base.Utilities;
using System;
using System.IO;

namespace WonderLab.Utilities;

public static class PathUtil {
    public static string GetDataFolderPath() => EnvironmentUtil.GetPlatformName() switch {
        "linux" => Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config", "WonderLab"),
        "osx" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WonderLab"),
        "windows" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WonderLab"),
        _ => throw new NotSupportedException()
    };

    public static string GetLogsFolderPath() => EnvironmentUtil.GetPlatformName() switch {
        "windows" => Path.Combine(Environment.CurrentDirectory, "WonderLab"),
        "linux" => Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".cache", "WonderLab"),
        "osx" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "WonderLab"),
        _ => throw new NotSupportedException()
    };
}