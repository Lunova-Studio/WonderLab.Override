namespace WonderLab.Classes.Models.Messaging;

public record RequestSearchMessage;

public record RequestDownloadPageGobackMessage;

public record RequestPageMessage(string S);

public record RequestResourcePageMessage(string Key, string ResourceName, object Parameter);