using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System.Collections.Specialized;
using System.Diagnostics;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "Title", "Gallery")]
[StyledProperty(typeof(string), "EmptyTip", "Empty")]
public sealed partial class GalleryView : ItemsControl {
    private StackPanel _PART_EmptyTipScrollPanel;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_EmptyTipScrollPanel = e.NameScope.Find<StackPanel>("PART_EmptyTipScrollPanel");

        Items.CollectionChanged += OnItemsCollectionChanged;
    }

    private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
                _PART_EmptyTipScrollPanel.IsVisible = Items.Count is 0;
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
            default:
                break;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        _PART_EmptyTipScrollPanel.IsVisible = Items.Count is 0;
    }
}