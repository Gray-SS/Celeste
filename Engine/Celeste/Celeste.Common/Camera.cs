using Microsoft.Xna.Framework;

namespace Celeste.Common;

public struct Camera
{
    public Color ClearColor { get; set; }

    public Camera()
    {
        ClearColor = Color.CornflowerBlue;
    }
}