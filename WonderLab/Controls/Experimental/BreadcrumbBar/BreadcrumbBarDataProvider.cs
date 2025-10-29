using Avalonia.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace WonderLab.Controls.Experimental.BreadcrumbBar;

internal sealed class BreadcrumbBarDataProvider : SplitDataSourceBase<object, BreadcrumbBarSplitVectorId, double> {
    private IEnumerable _rawDataSource;
    private ItemsSourceView _dataSource;
    private double _overflowButtonCachedWidth;
    private Action<NotifyCollectionChangedEventArgs> _dataChangedCallback;

    public BreadcrumbBarDataProvider(BreadcrumbBar owner) : base(5) {
        //Wow Microsoft, creative naming
        var primaryVector = new SplitVector<object, BreadcrumbBarSplitVectorId>(BreadcrumbBarSplitVectorId.PrimaryList, IndexOf);
        var secondaryVector = new SplitVector<object, BreadcrumbBarSplitVectorId>(BreadcrumbBarSplitVectorId.OverflowList, IndexOf);

        InitializeSplitVectors(primaryVector, secondaryVector);
    }

    public IList<object> GetPrimaryItems() =>
        GetVector(BreadcrumbBarSplitVectorId.PrimaryList).Vector;

    public IList<object> GetOverflowItems() =>
        GetVector(BreadcrumbBarSplitVectorId.OverflowList).Vector;

    // The raw data is from MenuItems
    public void SetDataSource(IEnumerable rawData) {
        if (ShouldChangeDataSource(rawData)) // avoid to create multiple of datasource for the same raw data
        {
            ItemsSourceView dataSource = null;
            if (rawData is not null) {
                //Avalonia ItemsSourceView only accepts IEnumerable types
                dataSource = ItemsSourceView.GetOrCreate(rawData);
            }

            ChangeDataSource(dataSource);
            _rawDataSource = rawData;
            if (dataSource is not null) {
                MoveAllItemsToPrimaryList();
            }
        }
    }

    public bool ShouldChangeDataSource(IEnumerable rawData) {
        return rawData != _rawDataSource;
    }

    public void OnRawDataChanged(Action<NotifyCollectionChangedEventArgs> dataChangedCallback) {
        _dataChangedCallback = dataChangedCallback;
    }

    public override int IndexOf(object value) {
        if (_dataSource != null) {
            return _dataSource.IndexOf(value);
        }
        return -1;
    }

    public int IndexOf(object value, BreadcrumbBarSplitVectorId id) {
        int indexInOriginalVector = IndexOf(value);
        int index = -1;
        if (indexInOriginalVector != -1) {
            var vector = GetVectorForItem(indexInOriginalVector);
            if (vector != null && vector.GetVectorIDForItem() == id) {
                index = vector.IndexFromIndexInOriginalVector(indexInOriginalVector);
            }
        }
        return index;
    }

    public override object GetAt(int index) {
        if (_dataSource != null)
            return _dataSource.GetAt(index);

        return null;
    }

    public override int Size {
        get {
            if (_dataSource != null)
                return _dataSource.Count;

            return 0;
        }
    }

    protected override double DefaultAttachedData => double.MinValue;
    protected override BreadcrumbBarSplitVectorId DefaultVectorIDOnInsert => BreadcrumbBarSplitVectorId.NotInitialized;

    public int PrimaryListSize => GetPrimaryItems().Count;

    public int NavigationViewItemCountInPrimaryList {
        get {
            int count = 0;
            for (int i = 0; i < Size; i++) {
                if (IsItemInPrimaryList(i)) {
                    count++;
                }
            }
            return count;
        }
    }

    public int NavigationViewItemCountInTopNav {
        get {
            int count = 0;
            //for (int i = 0; i < Size; i++) {
            //    if (IsContainerNavigationViewItem(i)) {
            //        count++;
            //    }
            //}
            return count;
        }
    }

    public void MoveAllItemsToPrimaryList() {
        for (int i = 0; i < Size; i++) {
            MoveItemToVector(i, BreadcrumbBarSplitVectorId.PrimaryList);
        }
    }

    public IList<int> ConvertPrimaryIndexToIndex(IList<int> indicesInPrimary) {
        List<int> indices = new List<int>();
        if (indicesInPrimary.Count != 0) {
            var vector = GetVector(BreadcrumbBarSplitVectorId.PrimaryList);
            if (vector != null) {
                //https://github.com/unoplatform/uno/blob/master/src/Uno.UI/UI/Xaml/Controls/NavigationView/TopNavigationViewDataProvider.cs
                indices.AddRange(indicesInPrimary.Select(vector.IndexToIndexInOriginalVector));
            }
        }
        return indices;
    }

    public int ConvertPrimaryIndexToIndex(int indexInPrimary) {
        var vector = GetVector(BreadcrumbBarSplitVectorId.PrimaryList);
        return vector.IndexToIndexInOriginalVector(indexInPrimary);
    }

    public int ConvertOverflowIndexToIndex(int indexInOverflow) {
        var vector = GetVector(BreadcrumbBarSplitVectorId.OverflowList);
        return vector.IndexToIndexInOriginalVector(indexInOverflow);
    }

    public int ConvertOriginalIndexToIndex(int originalIndex) {
        var vector = GetVector(IsItemInPrimaryList(originalIndex) ? BreadcrumbBarSplitVectorId.PrimaryList : BreadcrumbBarSplitVectorId.OverflowList);
        return vector.IndexFromIndexInOriginalVector(originalIndex);
    }

    public void MoveItemsOutOfPrimaryList(IList<int> indices) {
        MoveItemsToList(indices, BreadcrumbBarSplitVectorId.OverflowList);
    }

    public void MoveItemsToList(IList<int> indices, BreadcrumbBarSplitVectorId id) {
        foreach (var index in indices) {
            MoveItemToVector(index, id);
        }
    }

    public void UpdateWidthForPrimaryItem(int indexInPrimary, double width) {
        var vector = GetVector(BreadcrumbBarSplitVectorId.PrimaryList);
        if (vector != null) {
            var index = vector.IndexToIndexInOriginalVector(indexInPrimary);
            SetWidthForItem(index, width);
        }
    }

    public double WidthRequiredToRecoveryAllItemsToPrimary() {
        var width = 0.0;
        for (int i = 0; i < Size; i++) {
            if (!IsItemInPrimaryList(i)) {
                width += GetWidthForItem(i);
            }
        }
        width -= _overflowButtonCachedWidth;
        return Math.Max(0.0, width);
    }

    public bool HasInvalidWidth(IList<int> items) {
        bool hasInvalidWidth = false;
        foreach (var index in items) {
            if (!IsValidWidthForItem(index)) {
                hasInvalidWidth = true;
                break;
            }
        }
        return hasInvalidWidth;
    }

    public double GetWidthForItem(int index) {
        var width = AttachedData(index);
        if (!IsValidWidth(width)) {
            width = 0;
        }
        return width;
    }

    public double CalculateWidthForItems(IList<int> items) {
        double width = 0.0;
        foreach (var index in items) {
            width += GetWidthForItem(index);
        }
        return width;
    }

    public void InvalidWidthCache() {
        ResetAttachedData(-1.0);
    }

    public double OverflowButtonWidth {
        get => _overflowButtonCachedWidth;
        set => _overflowButtonCachedWidth = value;
    }

    public bool IsItemSelectableInPrimaryList(object value) {
        int index = IndexOf(value);
        return (index != -1);
    }

    public void OnDataSourceChanged(object sender, NotifyCollectionChangedEventArgs args) {
        switch (args.Action) {
            case NotifyCollectionChangedAction.Add when args.NewItems is not null:
                OnInsertAt(args.NewStartingIndex, args.NewItems.Count);
                break;
            case NotifyCollectionChangedAction.Remove when args.OldItems is not null:
                OnRemoveAt(args.OldStartingIndex, args.OldItems.Count);
                break;
            case NotifyCollectionChangedAction.Reset:
                OnClear();
                break;
            case NotifyCollectionChangedAction.Replace when args.NewItems is not null && args.OldItems is not null:
                OnRemoveAt(args.OldStartingIndex, args.OldItems.Count);
                OnInsertAt(args.NewStartingIndex, args.NewItems.Count);
                break;
        }
        _dataChangedCallback?.Invoke(args);
    }

    public bool IsValidWidth(double width) {
        return (width >= 0) && (width < double.MaxValue);
    }

    public bool IsValidWidthForItem(int index) {
        var width = AttachedData(index);
        return IsValidWidth(width);
    }

    public void SetWidthForItem(int index, double width) {
        if (IsValidWidth(width)) {
            AttachedData(index, width);
        }
    }

    public void ChangeDataSource(ItemsSourceView newValue) {
        var oldValue = _dataSource;
        if (oldValue != newValue) {
            //update to new datasource
            if (oldValue is INotifyCollectionChanged nc) {
                nc.CollectionChanged -= OnDataSourceChanged;
            }

            Clear();

            _dataSource = newValue;

            SyncAndInitVectorFlagsWithID(BreadcrumbBarSplitVectorId.NotInitialized, DefaultAttachedData);

            if (newValue is INotifyCollectionChanged newNC) {
                newNC.CollectionChanged += OnDataSourceChanged;
            }
        }

        // Move all to primary list
        MoveItemsToVector(BreadcrumbBarSplitVectorId.NotInitialized);
    }

    public bool IsItemInPrimaryList(int index) {
        return GetVectorIDForItem(index) == BreadcrumbBarSplitVectorId.PrimaryList;
    }

    public void MoveItemsToPrimaryList(IList<int> indexes) {
        MoveItemsToList(indexes, BreadcrumbBarSplitVectorId.PrimaryList);
    }
}

enum BreadcrumbBarSplitVectorId {
    NotInitialized = 0,
    PrimaryList,
    OverflowList,
    SkippedList,
    Size
}