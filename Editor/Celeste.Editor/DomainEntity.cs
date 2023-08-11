using Celeste.Common;
using Celeste.ECS;
using Celeste.Editor.Rendering;
using Celeste.ImGuiNET;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Celeste.Editor;

public class DomainEntityRenderer : ObjectVisualizer<DomainEntity>
{
    public DomainEntityRenderer(EditorRuntime editor) : base(editor)
    {
    }

    protected override void OnImGuiRender(DomainEntity target)
    {
        var selectionComponents = target.GetComponents();
        foreach (var component in selectionComponents)
        {
            var componentValueChanged = false;
            var type = component.GetType();
            if (!type.IsValueType) return;

            ImGui.SeparatorText(type.Name);

            foreach (var property in type.GetProperties())
            {
                var drawer = Editor.GetPropertyDrawer(property.PropertyType);
                if (drawer == null) continue;

                ImGui.TextColored(Color.LightBlue.ToNumerics(), property.Name);

                ImGui.SameLine();
                drawer.Render(GetHashCode(), component, property);

                if (drawer.ChangeTriggered(out object? newState))
                {
                    property.SetValue(component, newState);
                    componentValueChanged = true;
                }
            }

            if (componentValueChanged)
                target.SetComponent(component);
        }

        ImGui.Dummy(new Vector2(0, 10).ToNumerics());

        var mid = ImGui.GetWindowWidth() * 0.5f - 60f;
        ImGui.Dummy(new Vector2(mid, 0f).ToNumerics());
        ImGui.SameLine();

        ImGui.SetNextItemWidth(100f);

        if (ImGui.Button("Add Component"))
            ImGui.OpenPopup("add_comp");

        if (ImGui.BeginPopup("add_comp"))
        {
            var components = GameLogic.Instance.EcsRuntime.GetAliveComponentTypes();
            foreach (var component in components)
            {
                if (selectionComponents.Any(x => x.GetType() == component))
                    continue;

                if (ImGui.MenuItem(component.Name))
                {
                    var instance = Activator.CreateInstance(component)!;
                    target.AddComponent(instance);
                }
            }

            ImGui.EndPopup();
        }
    }
}

public class DomainEntity
{
    public string Name { get; set; }
    public Entity Entity { get; }

    private readonly EcsRuntime _ecs;

    public DomainEntity(string name, Entity entity, EcsRuntime ecsRuntime)
    {
        Name = name;
        Entity = entity;
        _ecs = ecsRuntime;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent<T>(T component)
        where T : struct
        => _ecs.AddComponent(Entity, component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComponent<T>(T component)
        where T : struct
        => _ecs.SetComponent(Entity, component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComponent(object component)
        => _ecs.SetComponent(Entity, component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveComponent<T>()
        where T : struct
        => _ecs.RemoveComponent<T>(Entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent<T>()
        where T : struct
        => ref _ecs.GetComponent<T>(Entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent<T>()
        where T : struct
        => _ecs.HasComponent<T>(Entity);

    public void AddComponent(object component)
        => _ecs.AddComponent(Entity, component);

    public IEnumerable<object> GetComponents()
        => _ecs.GetComponents(Entity);
}