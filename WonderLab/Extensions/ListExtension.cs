using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WonderLab.Extensions;

public static class ListExtension {
    public static ObservableCollection<T> ToObservableList<T>(this IEnumerable<T> list) {
        return new(list);
    }
}