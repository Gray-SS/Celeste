namespace Celeste.ECS;

public class EntityManager
{
    public IReadOnlyList<Entity> Entities => m_entities;

    private uint m_livingEntitiesCount;
    private readonly int m_maxEntities;
    private readonly Signature[] m_signatures;
    private readonly List<Entity> m_entities;
    private readonly Queue<Entity> m_availableEntities;

    public EntityManager(int maxEntities, int maxComponents)
    {
        m_maxEntities = maxEntities;
        m_signatures = new Signature[m_maxEntities];

        for (int i = 0; i < m_signatures.Length; i++)
            m_signatures[i] = new Signature(maxComponents);

        m_entities = new(m_maxEntities);
        m_availableEntities = new(m_maxEntities);
        for (uint i = 0; i < m_maxEntities; i++)
        {
            var entity = Entity.Create(i + 1);
            m_availableEntities.Enqueue(entity);
        }
    }

    public Entity CreateEntity()
    {
        if (m_livingEntitiesCount + 1 >= m_maxEntities)
            throw new InvalidOperationException("Too many entities in existence");

        Entity id = m_availableEntities.Dequeue();
        m_entities.Add(id);
        m_livingEntitiesCount++;

        return id;
    }

    public void DestroyEntity(Entity entity)
    {
        EnsureEntityValidity(entity);

        m_signatures[entity.id].Clear();

        m_availableEntities.Enqueue(entity);
        m_entities.Remove(entity);
        m_livingEntitiesCount--;
    }

    public void SetSignature(Entity entity, Signature signature)
    {
        EnsureEntityValidity(entity);
        m_signatures[entity.id] = signature;
    }

    public ref Signature GetSignature(Entity entity)
    {
        EnsureEntityValidity(entity);

        return ref m_signatures[entity.id];
    }

    private static void EnsureEntityValidity(Entity entity)
    {
        if (!entity.IsValid())
            throw new InvalidOperationException("Entity is not valid.");
    }
}
