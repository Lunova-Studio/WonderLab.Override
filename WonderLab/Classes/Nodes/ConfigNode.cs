using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WonderLab.Classes.Nodes;

public sealed class ConfigNode : IEnumerable<KeyValuePair<string, object>> {
    private readonly IDictionary<string, object> _entries =
        new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

    public object this[string key] {
        get => _entries[key];
        set => _entries[key] = value;
    }

    public bool TryGetValue(string key, out object value) => _entries.TryGetValue(key, out value);

    public T GetValue<T>(string key) where T : notnull {
        if (_entries.TryGetValue(key, out var value)) {
            if (value is T result)
                return result;

            throw new InvalidCastException($"Key '{key}' is not of type {typeof(T).Name}.");
        }

        return default;
    }

    public override string ToString() {
        var sb = new StringBuilder();
        foreach (var (key, value) in _entries) {
            sb.Append(key)
              .Append('=')
              .AppendLine(SerializeValue(value));
        }
        return sb.ToString();
    }

    public static ConfigNode Parse(string text) {
        var node = new ConfigNode();
        var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawLine in lines) {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#') || line.StartsWith("//"))
                continue;

            var idx = line.IndexOf('=');
            if (idx < 0) continue;

            var key = line[..idx].Trim();
            var valueStr = line[(idx + 1)..].Trim();

            node._entries[key] = ParseValue(valueStr);
        }

        return node;
    }

    private static object ParseValue(string valueStr) {
        if (string.Equals(valueStr, "null", StringComparison.OrdinalIgnoreCase))
            return null!;

        if (bool.TryParse(valueStr, out var boolResult))
            return boolResult;

        if (int.TryParse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intResult))
            return intResult;

        if (double.TryParse(valueStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleResult))
            return doubleResult;

        return valueStr;
    }

    private static string SerializeValue(object value) => value switch {
        null => "null",
        bool b => b.ToString().ToLowerInvariant(),
        string s when s.Contains(' ') || s.Contains('=') => s,

        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? ""
    };

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _entries.GetEnumerator();
}