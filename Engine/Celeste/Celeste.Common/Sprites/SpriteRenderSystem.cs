using Celeste.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Common.Sprites;
public class SpriteRenderSystem : EntitySystem
{
    private SpriteBatch _spriteBatch = null!;

    protected override void OnStart()
    {
        var graphics = GameLogic.Instance.GraphicsDevice;
        _spriteBatch = new SpriteBatch(graphics);
    }

    public override void Perform()
    {
        var query = Runtime.PerformQuery<Camera>();
        if (query.IsEmpty()) return;

        var cameraEntity = query.Single();
        ref var camera = ref Runtime.GetComponent<Camera>(cameraEntity);
        
        var graphicsDevice = GameLogic.Instance.GraphicsDevice;
        var viewport = graphicsDevice.Viewport;
        var cameraMatrix = Matrix.CreateTranslation(viewport.Width * 0.5f, viewport.Height * 0.5f, 0f);

        graphicsDevice.Clear(camera.ClearColor);

        _spriteBatch.Begin(transformMatrix: cameraMatrix);

        query = Runtime.PerformQuery<Sprite, Transform>();
        foreach (var entity in query)
        {
            ref Sprite sprite = ref Runtime.GetComponent<Sprite>(entity);
            ref Transform transform = ref Runtime.GetComponent<Transform>(entity);

            if (sprite.Texture == null)
                continue;

            var origin = sprite.Texture.Bounds.Size.ToVector2() * 0.5f;

            _spriteBatch.Draw(sprite.Texture, transform.Position, null, sprite.Color, transform.Rotation, origin, transform.Scale, 0, 0f);
        }

        _spriteBatch.End();
    }
}
