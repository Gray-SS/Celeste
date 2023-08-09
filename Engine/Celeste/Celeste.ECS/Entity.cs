using System.Diagnostics.CodeAnalysis;

namespace Celeste.ECS;

public readonly struct Entity
    : IEquatable<Entity>
{
    internal readonly uint id;

    public static readonly Entity Invalid = new(0);

    public bool IsValid()
        => this != Invalid;

    public Entity()
        => this = Invalid;

    private Entity(uint id)
        => this.id = id;

    internal static Entity Create(uint id)
        => new(id);

    public bool Equals(Entity other)
        => id == other.id;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Entity entity && Equals(entity);

    public override int GetHashCode()
        => HashCode.Combine(id);

    public static bool operator ==(Entity left, Entity right)
        => left.Equals(right);

    public static bool operator !=(Entity left, Entity right)
        => !left.Equals(right);

    public static implicit operator int(Entity entity)
        => (int)entity.id;

    public override string ToString()
        => $"{id}";
}
