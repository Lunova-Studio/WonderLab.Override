using WonderLab.Classes.Enums;

namespace WonderLab.Classes.Models.Messaging;

public record SearchResourceMessage(string Filter, SearchType SearchType);