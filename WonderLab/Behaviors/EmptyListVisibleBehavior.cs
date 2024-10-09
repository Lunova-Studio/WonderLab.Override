using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System.Collections;
using System.Collections.Specialized;

namespace WonderLab.Behaviors;

public sealed class EmptyListVisibleBehavior : Behavior {
    public bool IsObservableCollection => ItemsSource is INotifyCollectionChanged;

    public static readonly StyledProperty<IList> ItemsSourceProperty =
        AvaloniaProperty.Register<EmptyListVisibleBehavior, IList>(nameof(ItemsSource));

    public IList ItemsSource {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is Control control) {
            control.Loaded += OnLoaded;
            control.Unloaded += OnUnloaded;
        }
    }

    private void UpdateVisibility() {
        (AssociatedObject as Control).IsVisible = ItemsSource == null || ItemsSource.Count == 0;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) {
        if (IsObservableCollection) {
            (ItemsSource as INotifyCollectionChanged).CollectionChanged += OnCollectionChanged;
        }

        UpdateVisibility();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) {
        if (IsObservableCollection) {
            (ItemsSource as INotifyCollectionChanged).CollectionChanged -= OnCollectionChanged;
        }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        UpdateVisibility();
    }
}