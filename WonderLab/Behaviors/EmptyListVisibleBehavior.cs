using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System.Collections;
using System.Collections.Specialized;

namespace WonderLab.Behaviors;

public sealed class EmptyListVisibleBehavior : Behavior<Control> {
    public bool IsObservableCollection => ItemsSource is INotifyCollectionChanged;

    public static readonly StyledProperty<IList> ItemsSourceProperty =
        AvaloniaProperty.Register<EmptyListVisibleBehavior, IList>(nameof(ItemsSource));

    public IList ItemsSource {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    protected override void OnAttached() {
        base.OnAttached();

        if (IsObservableCollection) {
            (ItemsSource as INotifyCollectionChanged).CollectionChanged += OnCollectionChanged;
        }

        UpdateVisibility();
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (IsObservableCollection) {
            (ItemsSource as INotifyCollectionChanged).CollectionChanged -= OnCollectionChanged;
        }
    }

    private void UpdateVisibility() {
        AssociatedObject.IsVisible = ItemsSource == null || ItemsSource.Count == 0;
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        UpdateVisibility();
    }
}