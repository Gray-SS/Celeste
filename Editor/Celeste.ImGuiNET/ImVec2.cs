namespace Celeste.ImGuiNET;

public struct ImVec2
    : IEquatable<ImVec2>
{
    public float x;
    public float y;

    public static readonly ImVec2 Zero = new();

    public ImVec2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public ImVec2(float value) : this(value, value)
    {
    }

    public readonly bool Equals(ImVec2 other)
        => other.x == x && other.y == y;

    public override readonly bool Equals(object? obj)
    {
        return obj is ImVec2 vec && Equals(vec);
    }

    public static bool operator ==(ImVec2 left, ImVec2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ImVec2 left, ImVec2 right)
    {
        return !(left == right);
    }

    public override readonly int GetHashCode()
        => HashCode.Combine(x, y);

    public static implicit operator System.Numerics.Vector2(ImVec2 vec)
        => new(vec.x, vec.y);
}
