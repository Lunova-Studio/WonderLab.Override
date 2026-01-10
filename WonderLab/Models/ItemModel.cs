namespace WonderLab.Models;

public abstract record ItemModel {
    public string PageKey { get; set; }
    public string I18nKey { get; set; }
}

public record TabItemModel : ItemModel;

public record ListItemModel : ItemModel {
    public string Icon { get; set; }
}