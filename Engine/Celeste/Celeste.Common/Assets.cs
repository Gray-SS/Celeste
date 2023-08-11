using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Common;

public static class Assets
{
    public static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "Assets";

    private static readonly Dictionary<string, Texture2D> _textureCache = new();

    public static Texture2D LoadTexture(string resourcePath)
    {
        if (_textureCache.TryGetValue(resourcePath, out Texture2D? texture))
            return texture;

        var graphicsDevice = GameLogic.Instance.GraphicsDevice;
        var path = System.IO.Path.Combine(Path, resourcePath);

        if (!File.Exists(path)) throw new FileNotFoundException(path);

        try
        {
            using var stream = new FileStream(path, FileMode.Open);
            texture = Texture2D.FromStream(graphicsDevice, stream);
            _textureCache[resourcePath] = texture;

            return texture;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading texture: " + ex.Message);
            return null;
        }
    }
}
