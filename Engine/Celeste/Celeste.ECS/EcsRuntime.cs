using System.Runtime.CompilerServices;

namespace Celeste.ECS;

public partial class EcsRuntime
{
    public const int MAX_COMPONENTS = byte.MaxValue + 1;
    public const int MAX_ENTITIES = ushort.MaxValue + 1;

    private bool _initialized;
    private readonly SystemManager _systemManager;
    private readonly EntityManager _entityManager;
    private readonly ComponentManager _componentManager;

    public EcsRuntime()
    {
        _systemManager = new SystemManager();
        _componentManager = new ComponentManager(MAX_COMPONENTS, MAX_ENTITIES);
        _entityManager = new EntityManager(MAX_ENTITIES, MAX_COMPONENTS);
    }

    private void Initialize()
        => _systemManager.Initialize(this);

    public void Update()
    {
        if (!_initialized)
        {
            Initialize();
            _initialized = true;
        }

        _systemManager.Update();
    }

    public void Draw()
        => _systemManager.Render();

    public void AddComponent<T>(Entity entity, T component)
        where T : struct
    {
        _componentManager.AddComponent(entity, component);

        ref var signature = ref _entityManager.GetSignature(entity);
        signature.Flip(_componentManager.GetComponentType<T>().id);
    }

    public void RemoveComponent<T>(Entity entity)
        where T : struct
    {
        _componentManager.RemoveComponent<T>(entity);
    }

    public bool HasComponent<T>(Entity entity)
        where T : struct
    {
        return _componentManager.HasComponent<T>(entity);
    }

    public ref T GetComponent<T>(Entity entity)
        where T : struct
    {
        return ref _componentManager.GetComponentByRef<T>(entity);
    }

    public void SetComponent(Entity entity, object component)
    {
        _componentManager.SetComponent(entity, component);
    }

    public void SetComponent<T>(Entity entity, T component)
        where T : struct
    {
        _componentManager.SetComponent(entity, component);
    }

    public List<object> GetComponents(Entity entity)
    {
        var components = new List<object>();    

        Signature signature = _entityManager.GetSignature(entity);
        foreach(var componentType in _componentManager.ComponentTypes)
        {
            if (signature.IsBitSet(componentType.id))
            {
                var component = _componentManager.GetComponent(entity, componentType);
                components.Add(component);
            }
        }

        return components;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity CreateEntity()
        => _entityManager.CreateEntity();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DestroyEntity(Entity entity)
    {
        _entityManager.DestroyEntity(entity);
        _componentManager.EntityDestroyed(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddSystem(EntitySystem system)
        => _systemManager.AddSystem(ScheduleLabel.Update, system);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddSystem(ScheduleLabel schedule, EntitySystem system)
        => _systemManager.AddSystem(schedule, system);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveSystem(EntitySystem system)
        => _systemManager.RemoveSystem(ScheduleLabel.Update, system);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveSystem(ScheduleLabel schedule, EntitySystem system)
        => _systemManager.RemoveSystem(schedule, system);
}