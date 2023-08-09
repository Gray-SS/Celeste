namespace Celeste.ECS;

public enum ScheduleLabel
{
    Update,
    Render,
}

public class SystemManager
{
    private readonly Dictionary<ScheduleLabel, List<EntitySystem>> _systems = new();

    public void AddSystem(ScheduleLabel schedule, EntitySystem system)
    {
        if (!_systems.TryGetValue(schedule, out List<EntitySystem>? systems))
        {
            systems = new List<EntitySystem>();
            _systems.Add(schedule, systems);
        }

        systems.Add(system);
    }

    public void RemoveSystem(ScheduleLabel schedule, EntitySystem system)
    {
        if (!_systems.TryGetValue(schedule, out List<EntitySystem>? systems))
            return;

        systems.Remove(system);
    }

    internal void Initialize(EcsRuntime runtime)
    {
        foreach (var systemContainer in _systems.Values)
        {
            foreach (var system in systemContainer)
            {
                system.Init(runtime);
            }
        }
    }

    internal void Update()
    {
        if (_systems.TryGetValue(ScheduleLabel.Update, out List<EntitySystem>? systems))
        {
            foreach (var system in systems)
                system.Perform();
        }
    }

    internal void Render()
    {
        if (_systems.TryGetValue(ScheduleLabel.Render, out List<EntitySystem>? systems))
        {
            foreach (var system in systems)
                system.Perform();
        }
    }
}
