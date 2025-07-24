using Avalonia.Media;
using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Utilities;
using MinecraftLaunch.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Interfaces;
using WonderLab.Classes.Models;

namespace WonderLab.Classes.Importer;

/// <summary>
/// HMCL 设置导入器
/// </summary>
public sealed class HmclSettingImporter : ISettingImporter {
    public string Type => "HMCL";
    public string LauncherPath { get; set; }

    public async Task<SettingModel> ImportAsync() {
        var accountList = new List<Account>();
        var accountPath = GetAccountFilePath();

        if (string.IsNullOrWhiteSpace(accountPath) || !File.Exists(accountPath))
            throw new FileNotFoundException("未找到账户文件。", accountPath);

        var accountJson = await File.ReadAllTextAsync(accountPath).ConfigureAwait(false);
        var accountNodeList = accountJson.AsNode().GetEnumerable();

        foreach (var node in accountNodeList)
            accountList.Add(ParseHmclAccount(node));

        var baseDir = Path.GetDirectoryName(LauncherPath)
            ?? throw new DirectoryNotFoundException("启动器路径无效。");

        var configPath = Path.Combine(baseDir, "hmcl.json");
        if (!File.Exists(configPath))
            throw new FileNotFoundException("配置文件不存在。", configPath);

        var configJson = await File.ReadAllTextAsync(configPath).ConfigureAwait(false);
        var configNode = configJson.AsNode();

        // active account
        var selectedAccountStr = configNode.GetString("selectedAccount");
        var selectedUuid = selectedAccountStr?.Split(':').LastOrDefault();
        var activeAccount = Guid.TryParse(selectedUuid, out var uuid)
            ? accountList.FirstOrDefault(a => a.Uuid == uuid)
            : null;

        // portable accounts
        var portableNodes = configNode.GetEnumerable("accounts");
        foreach (var node in portableNodes)
            accountList.Add(ParseHmclAccount(node));

        var configMap = configNode.Select("configurations").AsObject();
        JsonNode activeGameSettings = null;
        string activeGameId = string.Empty;
        var minecraftFolders = new List<string>();

        foreach (var entry in configMap) {
            var value = entry.Value;
            if (value.GetBool("useRelativePath"))
                continue;

            var gameDir = value.GetString("gameDir");
            if (!string.IsNullOrEmpty(gameDir))
                minecraftFolders.Add(gameDir);

            var versionId = value.GetString("selectedMinecraftVersion");
            if (!string.IsNullOrEmpty(versionId)) {
                activeGameId = versionId;
                activeGameSettings = value.Select("global");
            }
        }

        if (activeGameSettings is null)
            throw new InvalidOperationException("未找到有效的游戏配置。");

        return new SettingModel {
            Accounts = accountList,
            ActiveAccount = activeAccount,
            ImagePath = configNode.GetString("bgpath"),
            MaxThread = configNode.GetInt32("downloadThreads"),
            IsEnableMirror = configNode.GetString("downloadType") == "bmclapi",
            ActiveColor = Color.Parse(configNode.GetString("theme")).ToUInt32(),
            ActiveBackground = ParseBackgroundType(configNode.GetString("backgroundType")),
            MaxMemorySize = activeGameSettings.GetInt32("maxMemory"),
            IsAutoMemory = activeGameSettings.GetBool("autoMemory"),
            Width = activeGameSettings.GetInt32("width"),
            Height = activeGameSettings.GetInt32("height"),
            ServerAddress = configNode.GetString("serverIp"),
            IsFullScreen = activeGameSettings.GetBool("fullscreen"),
            IsAutoSelectJava = activeGameSettings.GetString("java") == "Auto",
            ActiveGameId = activeGameId
        };
    }

    private static string GetAccountFilePath() {
        return EnvironmentUtil.IsWindow
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".hmcl", "accounts.json")
            : EnvironmentUtil.IsMac
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".hmcl", "accounts.json")
                : EnvironmentUtil.IsLinux
                    ? Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", ".local", "share", "hmcl", "accounts.json")
                    : throw new PlatformNotSupportedException("当前操作系统不支持");
    }

    private static Account ParseHmclAccount(JsonNode accountNode) {
        var type = accountNode.GetString("type");
        var uuid = Guid.Parse(accountNode.GetString("uuid"));

        return type switch {
            "offline" => new OfflineAccount(accountNode.GetString("username"), uuid, Guid.NewGuid().ToString("N")),
            "microsoft" => new MicrosoftAccount(accountNode.GetString("displayName"), uuid,
                accountNode.GetString("accessToken"), accountNode.GetString("refreshToken"), DateTime.UtcNow),
            "authlibInjector" => new YggdrasilAccount(accountNode.GetString("displayName"), uuid,
                accountNode.GetString("accessToken"), accountNode.GetString("serverBaseURL")),
            _ => throw new NotSupportedException($"不支持的账户类型：{type}")
        };
    }

    private static BackgroundType ParseBackgroundType(string type) => type switch {
        "CUSTOM" => BackgroundType.Bitmap,
        "CLASSIC" => BackgroundType.Voronoi,
        "DEFAULT" => BackgroundType.SolidColor,
        "TRANSLUCENT" => EnvironmentUtil.IsLinux ? BackgroundType.Bubble : BackgroundType.Acrylic,
        _ => BackgroundType.SolidColor
    };
}