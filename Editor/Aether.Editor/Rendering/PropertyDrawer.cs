using System.Reflection;

namespace Celeste.Editor.Rendering;

public abstract class PropertyDrawer
{
    public abstract Type TargetType { get; }

    internal abstract void RenderInternal(object target, PropertyInfo property);
}

public abstract class PropertyDrawer<T> : PropertyDrawer
    where T : struct
{
    public sealed override Type TargetType => typeof(T);

    private T _changedState;
    private bool _hasChanged;

    public bool ChangeTriggered(out T newState)
    {
        newState = _changedState;
        return _hasChanged;
    }

    protected void TriggerChanges(T change)
    {
        _hasChanged = true;
        _changedState = change;
    }

    protected abstract void Render(ref T value);

    internal override void RenderInternal(object target, PropertyInfo property)
    {
        if (property.PropertyType != TargetType)
            throw new InvalidOperationException();

        _hasChanged = false;
        _changedState = default;

        T value = (T)property.GetValue(target)!;
        var temp = value;
        Render(ref value);

        if (!temp.Equals(value)) TriggerChanges(value);
    }
}