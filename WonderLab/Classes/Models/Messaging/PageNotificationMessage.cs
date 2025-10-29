namespace WonderLab.Classes.Models.Messaging;

public record PageNotificationMessage(string PageKey);
public record DynamicPageNotificationMessage(string PageKey);
public record DynamicPageCloseNotificationMessage();