using Celeste.Editor.Utils;
using Celeste.ImGuiNET;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Celeste.Editor.Rendering;

internal class Vector2Drawer : PropertyDrawer<Vector2>
{
    protected override void Render(ref Vector2 value)
    {
        var id = ImGuiUtils.GetRandomId();
        var numerics = value.ToNumerics();

        if (ImGui.DragFloat2(id, ref numerics))
            value = numerics.ToXna();
    }
}