using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Celeste.Editor.Rendering;

public abstract class PropertyDrawer
{
    public abstract Type TargetType { get; }

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

    internal abstract void RenderInternal(int hashCode, object target, PropertyInfo property);
}

public abstract class PropertyDrawer<T> : PropertyDrawer
    where T : struct
{
    public sealed override Type TargetType => typeof(T);

    protected abstract void Render(string id, ref T value);

    internal override void RenderInternal(int hashCode, object target, PropertyInfo property)
    {
        if (property.PropertyType != TargetType)
            throw new InvalidOperationException();

        _hasChanged = false;
        _newState = null;

        T value = (T)property.GetValue(target)!;
        var temp = value;
        var id = $"##{HashCode.Combine(hashCode, property)}";
        Render(id, ref value);

        if (!temp.Equals(value)) TriggerChanges(value);
    }
}