using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Common;
using Celeste.ECS;
using Celeste.Common.Sprites;

namespace Celeste.Editor;

public class EditorGameLogic : GameLogic
{
    public EditorRuntime Editor { get; private set; } = null!;

    private RenderTarget2D _renderTarget = null!;

    public EditorGameLogic()
    {
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        Editor = new EditorRuntime(this);

        var cameraEntity = EcsRuntime.PerformQuery<Camera>().Single();
        Editor.BindEntity("Camera", cameraEntity);

        _renderTarget = new RenderTarget2D(GraphicsDevice, 1280, 720);

        DebugPurpose();
    }

    private void DebugPurpose()
    {
        var texture = new Texture2D(GraphicsDevice, 1, 1);
        texture.SetData(new Color[] { Color.White });

        var domainEntity = Editor.CreateEntity("Test");
        domainEntity.AddComponent(new Sprite(texture, Color.Orange));
        domainEntity.AddComponent(new Transform { Scale = new Vector2(100, 100) });
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.SetRenderTarget(_renderTarget);

        base.Draw(gameTime);

        GraphicsDevice.SetRenderTarget(null);

        Editor.OnImGuiRender(_renderTarget, gameTime);
    }
}
