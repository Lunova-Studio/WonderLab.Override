using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace WonderLab.UI.Controls;

///自带的太傻逼了索性重写
/// <summary>
/// A Toggle Switch control.
/// </summary>
public sealed class ToggleSwitch : ToggleButton {
    static ToggleSwitch() {
        IsCheckedProperty.Changed.AddClassHandler<ToggleSwitch>((x, x1) => {
            if ((x1.NewValue != null) && (x1.NewValue is bool val)) {
                x.UpdateHandlePos(val);
            }
        });
    }

    private void UpdateHandlePos(bool value) {

    }
}