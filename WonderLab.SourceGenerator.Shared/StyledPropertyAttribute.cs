namespace WonderLab;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class StyledPropertyAttribute(Type propertyType, string propertyName, object? defaultValue = default) : Attribute {
    public Type PropertyType { get; } = propertyType;
    public string PropertyName { get; } = propertyName;
    public object? DefaultValue { get; } = defaultValue;
}