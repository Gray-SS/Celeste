namespace Celeste.Editor;

public interface IEntitySelection
{
    IReadOnlyList<DomainEntity> SelectedEntities { get; }

    void Add(DomainEntity entity);
    void Remove(DomainEntity entity);
    void Clear();
}

internal class EntitySelection : IEntitySelection
{
    public IReadOnlyList<DomainEntity> SelectedEntities => _selectedEntities;

    private readonly List<DomainEntity> _selectedEntities = new();

    public void Add(DomainEntity entity)
    {
        _selectedEntities.Add(entity);
    }

    public void Remove(DomainEntity entity)
    {
        _selectedEntities.Remove(entity);
    }

    public void Clear()
    {
        _selectedEntities.Clear();
    }
}