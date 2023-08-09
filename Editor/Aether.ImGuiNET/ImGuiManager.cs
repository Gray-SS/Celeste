using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.ImGuiNET;

public class ImGuiManager
{
    private readonly ImGuiRenderer _renderer;
    private readonly Dictionary<Texture2D, nint> _texturesHandlers = new();

    public ImGuiManager(Game game)
    {
        _renderer = new ImGuiRenderer(game);
        _renderer.RebuildFontAtlas();
    }

    public nint GetOrCreateTexturePtr(Texture2D texture)
    {
        if (!_texturesHandlers.TryGetValue(texture, out var ptr))
        {
            ptr = _renderer.BindTexture(texture);
            _texturesHandlers.Add(texture, ptr);
        }

        return ptr;
    }

    public void BeginDraw(GameTime gameTime)
        => _renderer.BeforeLayout(gameTime);

    public void EndDraw()
        => _renderer.AfterLayout();
}
