using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Common.Sprites;

public struct Sprite
{
    public Texture2D Texture { get; set; }

    public Color Color { get; set; } = Color.White;

    public Sprite(Texture2D texture, Color? color = null)
    {
        Texture = texture;
        Color = color ?? Color.White;
    }
}