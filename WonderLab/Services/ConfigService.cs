using Microsoft.Extensions.Logging;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using System;
using System.IO;
using System.Text.Json;
using WonderLab.Infrastructure.Models;

namespace WonderLab.Services;

public sealed class ConfigService {
    private const string CONFIG_FILENAME = "wl_config.json";
    private readonly ILogger<ConfigService> _logger;

    public Config Entries { get; private set; }

    public ConfigService(ILogger<ConfigService> logger) {
        _logger = logger;
    }

    public void Load() {
        _logger.LogInformation("Loading config json");

        try {
            string json = File.ReadAllText(CONFIG_FILENAME);
            Entries = json.AsJsonEntry<Config>();
        } catch (Exception ex) {
            _logger.LogError("Failed to load config json.\n{Trace}", ex.ToString());

            Entries = new Config {
            };
        }

        if (string.IsNullOrEmpty(Entries.ActiveLanguage)) {
            Entries.ActiveLanguage = "zh-Hans";
        }

        if (Entries.Javas is null) {
            Entries.Javas = [];
        }


        if (Entries.MinecraftFolders is null) {
            Entries.MinecraftFolders = [];
        }
    }

    public void Save() {
        _logger.LogInformation("Saving config json");

        try {
            string json = Entries.AsJson();
            File.WriteAllText(CONFIG_FILENAME, json);
        } catch (Exception ex) {
            _logger.LogError("Failed to save config json.\n{Trace}", ex.ToString());
        }
    }
}