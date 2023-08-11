using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Celeste.Editor.Rendering;

public interface IPropertyDrawer
{
    Type PropertyType { get; }

    bool ChangeTriggered([NotNullWhen(true)] out object? newState);

    void Render(int hashCode, object target, PropertyInfo property);
}
