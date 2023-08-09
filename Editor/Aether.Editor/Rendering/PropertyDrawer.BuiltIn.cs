using ImGuiNET;
using Celeste.ImGuiNET;
using Microsoft.Xna.Framework;

namespace Celeste.Editor.Rendering;

internal class Vector2Drawer : PropertyDrawer<Vector2>
{
    protected override void Render(string id, ref Vector2 value)
    {
        var numerics = value.ToNumerics();

        if (ImGui.InputFloat2(id, ref numerics, $"%.2F", ImGuiInputTextFlags.EnterReturnsTrue))
            value = numerics.ToXna();
    }
}

internal class ColorDrawer : PropertyDrawer<Color>
{
    protected override void Render(string id, ref Color value)
    {
        var numerics = value.ToNumerics();

        if (ImGui.ColorEdit4(id, ref numerics))
            value = numerics.ToXna();
    }
}