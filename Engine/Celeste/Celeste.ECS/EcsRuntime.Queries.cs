namespace Celeste.ECS;

public partial class EcsRuntime
{
    public Query PerformQuery<T0>()
        where T0 : struct
    {
        var c0 = _componentManager.GetComponentType<T0>();

        var signature = new Signature(MAX_COMPONENTS);
        signature.Flip(c0.id);

        var entities = FetchEntities<T0>(signature);
        return new Query(entities);
    }

    public Query PerformQuery<T0, T1>()
        where T0 : struct
        where T1 : struct
    {
        var c0 = _componentManager.GetComponentType<T0>();
        var c1 = _componentManager.GetComponentType<T1>();

        var signature = new Signature(MAX_COMPONENTS);
        signature.Flip(c0.id);
        signature.Flip(c1.id);

        var entities = FetchEntities<T0>(signature);
        return new Query(entities);
    }

    public Query PerformQuery<T0, T1, T2>()
        where T0 : struct
        where T1 : struct
        where T2 : struct
    {
        var c0 = _componentManager.GetComponentType<T0>();
        var c1 = _componentManager.GetComponentType<T1>();
        var c2 = _componentManager.GetComponentType<T2>();

        var signature = new Signature(MAX_COMPONENTS);
        signature.Flip(c0.id);
        signature.Flip(c1.id);
        signature.Flip(c2.id);

        var entities = FetchEntities<T0>(signature);
        return new Query(entities);
    }

    public Query PerformQuery<T0, T1, T2, T3>()
        where T0 : struct
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        var c0 = _componentManager.GetComponentType<T0>();
        var c1 = _componentManager.GetComponentType<T1>();
        var c2 = _componentManager.GetComponentType<T2>();
        var c3 = _componentManager.GetComponentType<T3>();

        var signature = new Signature(MAX_COMPONENTS);
        signature.Flip(c0.id);
        signature.Flip(c1.id);
        signature.Flip(c2.id);
        signature.Flip(c3.id);

        var entities = FetchEntities<T0>(signature);
        return new Query(entities);
    }

    private IEnumerable<Entity> FetchEntities<T>(Signature query)
        where T : struct
    {
        foreach (var entity in _entityManager.Entities)
        {
            Signature signature = _entityManager.GetSignature(entity);
            if ((query & signature) == query)
            {
                yield return entity;
            }
        }
    }
}
