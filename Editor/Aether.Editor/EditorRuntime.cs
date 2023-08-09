using Celeste.Common;
using Celeste.ECS;
using Celeste.Editor.Rendering;
using Celeste.ImGuiNET;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Celeste.Editor;

public class EditorRuntime
{
    public IReadOnlyList<DomainEntity> Entities => _entities;

    public IEntitySelection EntitySelection => _entitySelection;

    private readonly GameLogic _game;
    private readonly List<DomainEntity> _entities;
    private readonly Queue<DomainEntity> _toDestroyEntities = new();
    private readonly List<PropertyDrawer> _propertyDrawers;

    private readonly IEntitySelection _entitySelection;

    public EditorRuntime(GameLogic game)
    {
        _game = game;
        _entities = new List<DomainEntity>();
        _entitySelection = new EntitySelection();

        _propertyDrawers = new List<PropertyDrawer>();
        foreach(var type in typeof(EditorRuntime).Assembly.GetTypes().Where(x => !x.IsInterface && !x.IsAbstract && x.IsAssignableTo(typeof(PropertyDrawer))))
        {
            var instance = (PropertyDrawer)Activator.CreateInstance(type)!;
            _propertyDrawers.Add(instance);
        }
    }

    public DomainEntity BindEntity(string name, Entity entity)
    {
        var domainEntity = new DomainEntity(name, entity, _game.EcsRuntime);
        _entities.Add(domainEntity);

        return domainEntity;
    }

    public void DestroyEntity(DomainEntity entity)
        => _toDestroyEntities.Enqueue(entity);

    public DomainEntity CreateEntity(string name)
    {
        var entity = _game.EcsRuntime.CreateEntity();
        var domainEntity = new DomainEntity(name, entity, _game.EcsRuntime);
        _entities.Add(domainEntity);

        return domainEntity;
    }

    public void ApplyChanges()
    {
        while (_toDestroyEntities.Count > 0)
        {
            var entity = _toDestroyEntities.Dequeue();
            _game.EcsRuntime.DestroyEntity(entity.Entity);
            _entities.Remove(entity);
        }
    }

    public void DrawPropertiesWindow()
    {
        ImGui.Begin("Properties");

        if (EntitySelection.SelectedEntities.Count > 0)
        {
            var selection = EntitySelection.SelectedEntities[0];
            var selectionComponents = selection.GetComponents();
            foreach (var component in selectionComponents)
            {
                var componentValueChanged = false;
                var type = component.GetType();
                if (!type.IsValueType) return;

                ImGui.SeparatorText(type.Name);

                foreach (var property in type.GetProperties())
                {
                    var drawer = _propertyDrawers.FirstOrDefault(x => x.TargetType == property.PropertyType);
                    if (drawer == null) continue;

                    ImGui.TextColored(Color.LightBlue.ToNumerics(), property.Name);

                    ImGui.SameLine();
                    drawer.RenderInternal(selection.GetHashCode(), component, property);

                    if (drawer.ChangeTriggered(out object? newState))
                    {
                        property.SetValue(component, newState);
                        componentValueChanged = true;
                    }
                }

                if (componentValueChanged)
                    selection.SetComponent(component);
            }

            ImGui.Dummy(new Vector2(0, 20).ToNumerics());

            if (ImGui.Button("Add Component"))
                ImGui.OpenPopup("add_comp");

            if (ImGui.BeginPopup("add_comp"))
            {
                var components = _game.EcsRuntime.GetAliveComponentTypes();
                foreach(var component in components)
                {
                    if (selectionComponents.Any(x => x.GetType() == component))
                        continue;

                    if (ImGui.MenuItem(component.Name))
                    {
                        var instance = Activator.CreateInstance(component)!;
                        selection.AddComponent(instance);
                    }
                }

                ImGui.EndPopup();
            }
        }

        ImGui.End();
    }
}