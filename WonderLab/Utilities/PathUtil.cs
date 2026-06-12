using MinecraftLaunch.Base.Utilities;
using System;
using System.IO;

namespace WonderLab.Utilities;

public static class PathUtil {
    private const string AppName = "WonderLab";

    public static string GetAppDataDirectory() => EnvironmentUtil.GetPlatformName() switch {
        "linux" => Path.Combine(GetLinuxDataDirectory(), AppName),
        "windows" => Path.Combine(AppContext.BaseDirectory, AppName),
        "osx" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName),
        _ => throw new NotSupportedException()
    };

    public static string GetLogsDirectory() => EnvironmentUtil.GetPlatformName() switch {
        "linux" => Path.Combine(GetLinuxCacheDirectory(), AppName, "logs"),
        "windows" => Path.Combine(AppContext.BaseDirectory, AppName, "logs"),
        "osx" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), AppName, "logs"),
        _ => throw new NotSupportedException()
    };
    
    private static string GetLinuxDataDirectory() => Environment.GetEnvironmentVariable("XDG_DATA_HOME")
        ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
    
    private static string GetLinuxCacheDirectory() => Environment.GetEnvironmentVariable("XDG_CACHE_HOME") 
        ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache");
}