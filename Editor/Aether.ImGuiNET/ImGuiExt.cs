namespace Celeste.ImGuiNET;

public static class ImGuiExt
{
    public static Microsoft.Xna.Framework.Vector2 ToNumerics(this System.Numerics.Vector2 value)
        => new(value.X, value.Y);

    public static System.Numerics.Vector2 ToXna(this System.Numerics.Vector2 value)
        => new(value.X, value.Y);
}
