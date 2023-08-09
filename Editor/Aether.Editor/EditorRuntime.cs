using Celeste.Common;
using Celeste.ECS;

namespace Celeste.Editor;

public class EditorRuntime
{
    public IReadOnlyList<DomainEntity> Entities => _entities;

    private readonly GameLogic _game;
    private readonly List<DomainEntity> _entities;
    private readonly Queue<DomainEntity> _toDestroyEntities = new();

    public EditorRuntime(GameLogic game)
    {
        _game = game;
        _entities = new List<DomainEntity>();
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
}