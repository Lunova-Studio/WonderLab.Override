using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "Text", "Tag")]
public sealed partial class Tag : TemplatedControl;

public sealed class Tags : ItemsControl;