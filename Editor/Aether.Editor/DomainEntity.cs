using Celeste.ECS;
using System.Runtime.CompilerServices;

namespace Celeste.Editor;

public class DomainEntity
{
    public string Name { get; set; }
    public Entity Entity { get; }

    private readonly EcsRuntime _ecsRuntime;

    public DomainEntity(string name, Entity entity, EcsRuntime ecsRuntime)
    {
        Name = name;
        Entity = entity;

        _ecsRuntime = ecsRuntime;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent<T>(T component)
        where T : struct
        => _ecsRuntime.AddComponent(Entity, component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComponent<T>(T component)
        where T : struct
        => _ecsRuntime.SetComponent(Entity, component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComponent(object component)
        => _ecsRuntime.SetComponent(Entity, component);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveComponent<T>()
        where T : struct
        => _ecsRuntime.RemoveComponent<T>(Entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetComponent<T>()
        where T : struct
        => ref _ecsRuntime.GetComponent<T>(Entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent<T>()
        where T : struct
        => _ecsRuntime.HasComponent<T>(Entity);

    public void AddComponent(object component)
        => _ecsRuntime.AddComponent(Entity, component);

    public IEnumerable<object> GetComponents()
        => _ecsRuntime.GetComponents(Entity);
}