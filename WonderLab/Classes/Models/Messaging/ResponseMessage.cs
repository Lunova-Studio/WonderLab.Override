using MinecraftLaunch.Base.Models.Network;

namespace WonderLab.Classes.Models.Messaging;

public record MinecraftResponseMessage(string MinecraftId, VersionManifestEntry MinecraftData);