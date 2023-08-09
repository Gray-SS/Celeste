namespace Celeste.ECS;

public interface IComponentHandler
{
    void Add(Entity entity, object component);

    void Remove(Entity entity);

    object Get(Entity entity);

    bool Has(Entity entity);

    void Set(Entity entity, object component);

    void EntityDestroyed(Entity entity);
}

public class ComponentHandler<T> : IComponentHandler
    where T : struct
{
    private int m_length;
    private readonly T[] m_data;
    private readonly int[] m_entityToIndex;
    private readonly Entity[] m_indexToEntity;

    public ComponentHandler(int maxEntities)
    {
        m_length = 0;
        m_data = new T[maxEntities];
        m_entityToIndex = new int[maxEntities];
        m_indexToEntity = new Entity[maxEntities];
    }

    public void Add(Entity entity, object component)
    {
        EnsureEntityValidity(entity);

        if (Has(entity)) throw new InvalidOperationException("This entity already have a component of this type assigned.");

        m_data[m_length] = (T)component;
        m_indexToEntity[m_length] = entity;
        m_entityToIndex[entity] = m_length + 1;
        m_length++;
    }

    public void Add(Entity entity, T component)
    {
        EnsureEntityValidity(entity);

        if (Has(entity)) throw new InvalidOperationException("This entity already have a component of this type assigned.");

        m_data[m_length] = component;
        m_indexToEntity[m_length] = entity;
        m_entityToIndex[entity] = m_length + 1;
        m_length++;
    }

    public void Set(Entity entity, T component)
    {
        EnsureEntityValidity(entity);

        m_data[m_length] = component;
        m_indexToEntity[m_length] = entity;
        m_entityToIndex[entity] = m_length + 1;
        m_length++;
    }

    public void Set(Entity entity, object component)
    {
        m_data[m_length] = (T)component;
        m_indexToEntity[m_length] = entity;
        m_entityToIndex[entity] = m_length + 1;
        m_length++;
    }

    public void Remove(Entity entity)
    {
        EnsureEntityValidity(entity);

        if (!Has(entity)) throw new InvalidOperationException("You're trying to remove a component that doesn't exist");

        var indexOfRemovedEntity = m_entityToIndex[entity] - 1;
        var indexOfLastElement = m_length - 1;
        m_data[indexOfRemovedEntity] = m_data[indexOfLastElement];

        Entity entityOfLastElement = m_indexToEntity[indexOfLastElement];
        m_entityToIndex[entityOfLastElement] = indexOfRemovedEntity;
        m_indexToEntity[indexOfRemovedEntity] = entityOfLastElement;

        m_entityToIndex[entity] = 0;
        m_indexToEntity[indexOfLastElement] = Entity.Invalid;

        m_length--;
    }

    public object Get(Entity entity)
    {
        EnsureEntityValidity(entity);

        if (!Has(entity))
            throw new InvalidOperationException("Retrieving non-existent component.");

        return m_data[m_entityToIndex[entity] - 1];
    }

    public ref T GetByRef(Entity entity)
    {
        EnsureEntityValidity(entity);

        if (!Has(entity))
            throw new InvalidOperationException("Retrieving non-existent component.");

        return ref m_data[m_entityToIndex[entity] - 1];
    }

    public bool Has(Entity entity)
    {
        EnsureEntityValidity(entity);
        return m_entityToIndex[entity] != 0;
    }

    public void EntityDestroyed(Entity entity)
    {
        if (Has(entity)) Remove(entity);
    }

    private static void EnsureEntityValidity(Entity entity)
    {
        if (!entity.IsValid())
            throw new InvalidOperationException("Entity is not valid");
    }
}
