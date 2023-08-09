namespace Celeste.ECS;

public class ComponentManager
{
    public int ComponentTypesCount => m_componentTypesCount;
    public IEnumerable<ComponentType> ComponentTypes => m_componentTypes.Values;

    private int m_componentTypesCount = 0;

    private readonly int m_maxEntities;

    private readonly Dictionary<int, ComponentType> m_componentTypes;
    private readonly Dictionary<ComponentType, IComponentHandler> m_componentHandlers;

    public ComponentManager(int maxComponents, int maxEntities)
    {
        m_maxEntities = maxEntities;

        m_componentTypes = new Dictionary<int, ComponentType>(maxComponents);
        m_componentHandlers = new Dictionary<ComponentType, IComponentHandler>(maxComponents);
    }

    public void AddComponent<T>(Entity entity, T component)
        where T : struct
    {
        var handler = GetComponentHandler<T>();
        handler.Add(entity, component);
    }

    public bool HasComponent<T>(Entity entity)
        where T : struct
    {
        var handler = GetComponentHandler<T>();
        return handler.Has(entity);
    }

    public void RemoveComponent<T>(Entity entity)
        where T : struct
    {
        var handler = GetComponentHandler<T>();
        handler.Remove(entity);
    }

    public void SetComponent<T>(Entity entity, T component)
        where T : struct
    {
        var handler = GetComponentHandler<T>();
        handler.Set(entity, component);
    }

    public void SetComponent(Entity entity, object component)
    {
        var componentType = GetComponentType(component.GetType());
        var handler = GetComponentHandler(componentType);
        handler.Set(entity, component);
    }

    public ref T GetComponentByRef<T>(Entity entity)
        where T : struct
    {
        var handler = GetComponentHandler<T>();
        return ref handler.GetByRef(entity);
    }

    public object GetComponent(Entity entity, ComponentType type)
    {
        var handler = GetComponentHandler(type);
        return handler.Get(entity);
    }

    public void EntityDestroyed(Entity entity)
    {
        foreach (var handler in m_componentHandlers.Values)
        {
            handler.EntityDestroyed(entity);
        }
    }

    private ComponentHandler<T> RegisterComponentHandler<T>()
        where T : struct
    {
        var componentType = GetComponentType<T>();

        var handler = new ComponentHandler<T>(m_maxEntities);
        m_componentHandlers[componentType] = handler;
        return handler;
    }

    public ComponentType GetComponentType(Type type)
    {
        var key = type.GetHashCode();
        if (m_componentTypes.TryGetValue(key, out ComponentType compType))
            return compType;

        throw new InvalidOperationException($"Component type with type '{type.FullName}' doesn't exists");
    }

    public ComponentType GetComponentType<T>()
    {
        var key = typeof(T).GetHashCode();
        if (m_componentTypes.TryGetValue(key, out ComponentType type))
            return type;

        var componentType = ComponentType.Create(m_componentTypesCount + 1);
        m_componentTypes[key] = componentType;
        m_componentTypesCount++;

        return componentType;
    }

    private ComponentHandler<T> GetComponentHandler<T>()
        where T : struct
    {
        var key = typeof(T).GetHashCode();
        if (m_componentTypes.TryGetValue(key, out ComponentType type))
            return (ComponentHandler<T>)m_componentHandlers[type];

        return RegisterComponentHandler<T>();
    }

    private IComponentHandler GetComponentHandler(ComponentType type)
    {
        if (m_componentHandlers.TryGetValue(type, out IComponentHandler? handler))
            return handler;

        throw new InvalidOperationException($"{type} isn't registered");
    }
}
