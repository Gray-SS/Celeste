using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Celeste.Editor.Rendering;

public abstract class PropertyDrawer : IPropertyDrawer
{
    public abstract Type PropertyType { get; }

    protected bool _hasChanged;
    protected object? _newState;

    public bool ChangeTriggered([NotNullWhen(true)] out object? newState)
    {
        newState = _newState;
        return _hasChanged;
    }

    protected void TriggerChanges(object newState)
    {
        _hasChanged = true;
        _newState = newState;
    }

    public abstract void Render(int hashCode, object target, PropertyInfo property);
}

public abstract class PropertyDrawer<T> : PropertyDrawer
{
    public sealed override Type PropertyType => typeof(T);

    protected abstract void Draw(string id, ref T value);

    public override void Render(int hashCode, object target, PropertyInfo property)
    {
        if (property.PropertyType != PropertyType)
            throw new InvalidOperationException();

        _hasChanged = false;
        _newState = null;

        T value = (T)property.GetValue(target)!;
        var temp = value;
        var id = $"##{HashCode.Combine(hashCode, property)}";
        Draw(id, ref value);

        if (!temp.Equals(value)) TriggerChanges(value);
    }
}