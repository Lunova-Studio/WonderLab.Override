using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WonderLab.SourceGenerator.Extensions;

internal static class IncrementalValuesProviderExtension {
    public static IncrementalValuesProvider<(TKey key, ImmutableArray<TValue> values)> GroupBy<TElement, TKey, TValue>(
        this IncrementalValuesProvider<TElement> source,
        Func<TElement, TKey> keySelector,
        Func<TElement, TValue> valueSelector)
        where TKey : IEquatable<TKey> {
        return source.Collect().SelectMany((item, token) => {
            var map = new Dictionary<TKey, ImmutableArray<TValue>.Builder>();

            foreach (var ele in item) {
                var key = keySelector(ele);
                var value = valueSelector(ele);

                if (!map.TryGetValue(key, out var builder)) {
                    builder = ImmutableArray.CreateBuilder<TValue>();
                    map.Add(key, builder);
                }

                builder.Add(value);
            }

            token.ThrowIfCancellationRequested();
            var result = ImmutableArray.CreateBuilder<(TKey, ImmutableArray<TValue>)>();
            foreach (var entry in map)
                result.Add((entry.Key, entry.Value.ToImmutable()));

            return result;
        });
    }
}