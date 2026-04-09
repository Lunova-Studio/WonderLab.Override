using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Material.Icons;
using System.Windows.Input;

namespace WonderLab.UI.Controls;

[PseudoClasses(":pressed")]
[StyledProperty(typeof(ICommand), "Command")]
[StyledProperty(typeof(string), "Header", "Hello")]
[StyledProperty(typeof(MaterialIconKind), "IconKind")]
public sealed partial class SettingsCardItem : ContentControl {
    static SettingsCardItem() {
        PressedMixin.Attach<SettingsCardItem>();
    }
}