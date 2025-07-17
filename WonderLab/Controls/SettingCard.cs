using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "Header", "Header")]
public sealed partial class SettingCards : ItemsControl;

[StyledProperty(typeof(string), "Icon")]
[StyledProperty(typeof(string), "Header", "Header")]
public sealed partial class SettingCard : ContentControl;

[StyledProperty(typeof(string), "Icon")]
[StyledProperty(typeof(object), "Footer")]
[StyledProperty(typeof(object), "Header", "Header")]
[StyledProperty(typeof(bool), "IsExpanded")]
[StyledProperty(typeof(bool), "CanExpanded")]
public sealed partial class SettingExpandCard : ItemsControl;