using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using System.Windows.Input;

namespace WonderLab.UI.Controls;

[StyledProperty(typeof(ICommand), "FABCommand")]
[TemplatePart("PART_ScrollViewer", typeof(IScrollable))]
public sealed partial class NavigationRail : ListBox {
    protected override Control CreateContainerForItemOverride(object item, int index, object recycleKey) {
        return new NavigationItem();
    }

    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey) {
        return NeedsContainer<NavigationItem>(item, out recycleKey);
    }
}