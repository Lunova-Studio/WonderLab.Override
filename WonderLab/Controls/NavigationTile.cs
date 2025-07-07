using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(object), "Icon")]
[StyledProperty(typeof(object), "Footer")]
[StyledProperty(typeof(string), "Title")]
[StyledProperty(typeof(object), "Description")]
[StyledProperty(typeof(bool), "IsDescriptionVisible", true)]
public sealed partial class NavigationTile : Tile;