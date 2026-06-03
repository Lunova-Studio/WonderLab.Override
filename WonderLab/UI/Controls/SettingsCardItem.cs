using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Media;
using Material.Icons;
using System.Windows.Input;
using Avalonia;

namespace WonderLab.UI.Controls;

[PseudoClasses(":pressed")]
[StyledProperty(typeof(ICommand), "Command")]
[StyledProperty(typeof(IBrush), "IconForeground")]
[StyledProperty(typeof(double), "IconSize")]
[StyledProperty(typeof(string), "Header", "Hello")]
[StyledProperty(typeof(object), "CommandParameter")]
[StyledProperty(typeof(MaterialIconKind), "IconKind")]
public sealed partial class SettingsCardItem : ContentControl {
    static SettingsCardItem() {
        PressedMixin.Attach<SettingsCardItem>();
    }
}