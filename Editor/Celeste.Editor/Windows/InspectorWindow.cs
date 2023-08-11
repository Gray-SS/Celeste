using Celeste.ECS;
using Celeste.ImGuiNET;
using ImGuiNET;

namespace Celeste.Editor.Windows;

public class InspectorWindow : EditorWindow
{
    public InspectorWindow(EditorRuntime runtime) : base(runtime)
    {
    }

    public override void OnImGuiRender()
    {
        ImGui.Begin("Inspector");

        ImGui.BeginChild("child");

        var canShowCreatePopup = true;

        var count = 0;

        foreach (var entity in Runtime.Entities)
        {
            if (ImGui.Selectable(entity.Name))
            {
                var win = Runtime.GetWindow<PropertiesWindow>();
                win.SetTarget(entity);
            }

            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.MenuItem("Delete"))
                {
                    Runtime.DestroyEntity(entity);
                }

                canShowCreatePopup = false;

                ImGui.EndPopup();
            }

            count++;
        }

        if (canShowCreatePopup && ImGui.BeginPopupContextWindow())
        {
            if (ImGui.BeginMenu("Create"))
            {
                if (ImGui.MenuItem("Empty")) Runtime.CreateEntity("Empty");
            }

            ImGui.EndPopup();
        }

        ImGui.EndChild();

        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGuiUtils.AcceptDragDropPayload<int>("drag-file-to-inspector");
            if (payload != null)
            {
                Console.WriteLine(payload);
            }

            ImGui.EndDragDropTarget();
        }

        ImGui.End();

        Runtime.ApplyChanges();
    }
}
