using Celeste.ECS;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Celeste.Editor.Utils;

public static class ImGuiUtils
{
    public static void DrawComponentProperties(EcsRuntime runtime, Entity entity, object component)
    {
        var type = component.GetType();
        if (!type.IsValueType) return;

        ImGui.SeparatorText(type.Name);

        foreach (var property in type.GetProperties())
        {
            DrawProperty(runtime, entity, component, property);
        }
    }

    public static string GetRandomId()
    {
        var value = new char[18];
        value[0] = '#';
        value[1] = '#';

        for (int i = 0; i < 16; i++)
        {
            value[i + 2] = (char)Random.Shared.Next(10);
        }

        return new string(value);
    }

    private static void DrawProperty(EcsRuntime runtime, Entity entity, object component, PropertyInfo property)
    {
        var id = $"##{property.GetHashCode()}";
        var propertyType = property.PropertyType;

        var triggeredChanges = false;

        if (propertyType == typeof(Vector2))
        {
            var xna = (Vector2)property.GetValue(component)!;

        }
        else if (propertyType == typeof(float))
        {
            var value = (float)property.GetValue(component)!;

        }

        if (triggeredChanges)
            runtime.SetComponent(entity, component);
    }
}
