using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "Glyph", "")]
public sealed partial class FontIcon : TemplatedControl;
