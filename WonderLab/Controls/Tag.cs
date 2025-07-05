using Avalonia.Controls.Primitives;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "Text", "Test")]
public sealed partial class Tag : TemplatedControl {
}