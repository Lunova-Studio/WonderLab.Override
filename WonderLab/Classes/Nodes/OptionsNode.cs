using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WonderLab.Classes.Nodes;

public partial class OptionsNode : IEnumerable<KeyValuePair<string, object>> {
    private readonly Dictionary<string, object> _entries = [];

    public object this[string key] {
        get => _entries[key];
        set => _entries[key] = value;
    }

    [GeneratedRegex("\"([^\"]*)\"")]
    protected static partial Regex OptionListRegex();

    internal OptionsNode() { }

    public bool TryGetValue(string key, out object value) => _entries.TryGetValue(key, out value);

    public static OptionsNode Parse(string text) {
        var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        return Parse(lines);
    }

    public static OptionsNode Parse(IEnumerable<string> lines) {
        var node = new OptionsNode();
        foreach (var rawLine in lines) {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var idx = line.IndexOf(':');
            if (idx < 0) continue;

            var key = line[..idx].Trim();
            var valueStr = line[(idx + 1)..].Trim();

            node._entries[key] = ParseValue(valueStr);
        }

        return node;
    }

    public T GetValue<T>(string key) {
        if (_entries.TryGetValue(key, out var value)) {
            if (value is T typed)
                return typed;

            throw new InvalidCastException($"Key '{key}' is not of type {typeof(T).Name}.");
        }

        if (!TryAddNode(key, default(T)!))
            throw new KeyNotFoundException($"Key '{key}' was not found and could not be defaulted.");

        return default!;
    }

    public bool TryAddNode<T>(string key, T value) => _entries.TryAdd(key, value!);

    public override string ToString() {
        var sb = new StringBuilder(_entries.Count * 32);
        foreach (var (key, value) in _entries) {
            sb.Append(key)
              .Append(':')
              .AppendLine(SerializeValue(value));
        }

        return sb.ToString();
    }

    private static object ParseValue(string valueStr) {
        if (valueStr.StartsWith('[') && valueStr.EndsWith(']')) {
            var inner = valueStr[1..^1].Trim();
            if (string.IsNullOrWhiteSpace(inner))
                return Array.Empty<string>();

            var items = OptionListRegex().Matches(inner).Select(m => m.Groups[1].Value).ToList();
            return items;
        }

        if (valueStr.Length >= 2 && valueStr[0] == '"' && valueStr[^1] == '"')
            return valueStr[1..^1];

        if (bool.TryParse(valueStr, out var b))
            return b;

        if (int.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var i))
            return i;

        if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            return d;

        return valueStr;
    }

    private static string SerializeValue(object value) => value switch {
        null => "",
        string s => $"\"{s}\"",
        bool b => b.ToString().ToLowerInvariant(),
        IEnumerable enumerable when value is not string => $"[{string.Join(",", enumerable.Cast<object>().Select(item => $"\"{item}\""))}]",
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? ""
    };

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _entries.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}