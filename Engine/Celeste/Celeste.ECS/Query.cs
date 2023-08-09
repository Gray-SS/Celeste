namespace Celeste.ECS;

public readonly struct Query
{
    private readonly IEnumerable<Entity> _entities;

    public Query(IEnumerable<Entity> entities)
        => _entities = entities;

    public bool IsEmpty()
        => !_entities.Any();

    public Entity Single()
        => _entities.First();

    public IEnumerator<Entity> GetEnumerator()
        => _entities.GetEnumerator();
}
