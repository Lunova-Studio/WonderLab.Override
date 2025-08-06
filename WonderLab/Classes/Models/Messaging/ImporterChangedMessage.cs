using WonderLab.Classes.Interfaces;

namespace WonderLab.Classes.Models.Messaging;

public record ImporterChangedMessage(ISettingImporter SettingImporter);