using Microsoft.Win32;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using WonderLab.Classes.Models;
using WonderLab.Classes.Nodes;

namespace WonderLab.Classes.Importer;

/// <summary>
/// PCL2 主线版设置导入器
/// </summary>
[SupportedOSPlatform("Windows")]
public sealed class PclSettingImporter : ISettingImporter {
    public string Type => "PCL";

    public string LauncherPath { get; set; }

    public async Task<(SettingModel Settings, bool IsSuccess)> ImportAsync() {
        if (string.IsNullOrWhiteSpace(LauncherPath) || !File.Exists(LauncherPath))
            return (null, false);

		try {
            var baseDir = Path.GetDirectoryName(LauncherPath);
            var settingsFilePath = Path.Combine(baseDir, "PCL", "Setup.ini");
            if (!File.Exists(settingsFilePath))
                return (null, false);

            var settingsContent = await File.ReadAllTextAsync(settingsFilePath)
                .ConfigureAwait(false);

            var node = OptionsNode.Parse(settingsContent);
            using var pclRegistry = Registry.CurrentUser.OpenSubKey(@"Software\PCL")
                ?? throw new InvalidOperationException("注册表键 'Software\\PCL' 未找到。");

            var rawJavaList = pclRegistry.GetValue("LaunchArgumentJavaAll")?.ToString();
            var javaNodeList = rawJavaList.AsNode()
                .GetEnumerable();

            var javaPaths = new List<string>();
            foreach (var javaNode in javaNodeList) {
                var path = javaNode.GetString("Path");

                if (Directory.Exists(path))
                    javaPaths.Add(Path.Combine(path, "javaw.exe"));
            }

            var javaEntries = new List<JavaEntry>();
            foreach (var path in javaPaths) {
                try {
                    var javaInfo = await JavaUtil.GetJavaInfoAsync(path)
                        .ConfigureAwait(false);

                    if (javaInfo is not null)
                        javaEntries.Add(javaInfo);
                } catch (Exception) { }
            }

            var minecraftFolders = pclRegistry.GetValue("LaunchFolders")
                ?.ToString()
                ?.Split('|')
                ?.Select(x => x.Split('>').Last());

            var minecraftId = node.GetValue<string>("LaunchVersionSelect");
            var minecraftFolder = node.GetValue<string>("LaunchFolderSelect");
            var settings = new SettingModel {
                ActiveGameId = minecraftId,
                ActiveMinecraftFolder = minecraftFolder,
                Javas = [.. javaEntries],
                MinecraftFolders = [.. minecraftFolders],
                IsAutoMemory = node.GetValue<int>("LaunchRamType") is 0,
                IsFullScreen = node.GetValue<int>("LaunchArgumentWindowType") is 0
            };

            return (settings, true);
        } catch (Exception) {
            return (null, false);
        }
    }
}