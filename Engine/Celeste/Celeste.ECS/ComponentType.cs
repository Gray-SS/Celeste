using System.Diagnostics.CodeAnalysis;

namespace Celeste.ECS;

public readonly struct ComponentType
    : IEquatable<ComponentType>
{
    internal readonly int id;

    public static readonly ComponentType Invalid = new(0);

    public bool IsValid()
        => this != Invalid;

    public ComponentType()
        => this = Invalid;

    private ComponentType(int id)
        => this.id = id;

    internal static ComponentType Create(int id)
        => new(id);

    public bool Equals(ComponentType other)
        => id == other.id;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is ComponentType entity && Equals(entity);

    public override int GetHashCode()
        => HashCode.Combine(id);

    public static bool operator ==(ComponentType left, ComponentType right)
        => left.Equals(right);

    public static bool operator !=(ComponentType left, ComponentType right)
        => !left.Equals(right);
}
