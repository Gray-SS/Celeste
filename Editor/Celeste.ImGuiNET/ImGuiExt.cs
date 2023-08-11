namespace Celeste.ImGuiNET;

public static class ImGuiExt
{
    public static Microsoft.Xna.Framework.Vector2 ToXna(this System.Numerics.Vector2 value)
        => new(value.X, value.Y);

    public static System.Numerics.Vector2 ToNumerics(this Microsoft.Xna.Framework.Vector2 value)
        => new(value.X, value.Y);

    public static Microsoft.Xna.Framework.Color ToXna(this System.Numerics.Vector4 value)
        => new(value.X, value.Y, value.Z, value.W);

    public static System.Numerics.Vector4 ToNumerics(this Microsoft.Xna.Framework.Color value)
        => new(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
}
