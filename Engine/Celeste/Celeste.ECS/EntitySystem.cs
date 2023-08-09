namespace Celeste.ECS;

public abstract class EntitySystem
{
    protected EcsRuntime Runtime { get; private set; } = null!;

    internal void Init(EcsRuntime ecsRuntime)
    {
        Runtime = ecsRuntime;
        OnStart();
    }

    protected virtual void OnStart()
    {
    }

    public abstract void Perform();
}