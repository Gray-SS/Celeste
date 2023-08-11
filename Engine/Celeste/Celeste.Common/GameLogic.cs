using Celeste.Common.Sprites;
using Celeste.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Common;

public class GameLogic : Game
{
    public GraphicsDeviceManager DeviceManager { get; }

    public EcsRuntime EcsRuntime { get; private set; } = null!;
    public SpriteBatch SpriteBatch { get; private set; } = null!;

    public static GameLogic Instance { get; private set; } = null!;

    public GameLogic()
    {
        Instance = this;
        DeviceManager = new GraphicsDeviceManager(this);

        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        DeviceManager.PreferredBackBufferWidth = 1280;
        DeviceManager.PreferredBackBufferHeight = 720;
        DeviceManager.ApplyChanges();

        EcsRuntime = new EcsRuntime();
        EcsRuntime.AddSystem(ScheduleLabel.Render, new SpriteRenderSystem());

        var camera = EcsRuntime.CreateEntity();
        EcsRuntime.AddComponent(camera, new Transform());
        EcsRuntime.AddComponent(camera, new Camera());
    }

    protected override void Update(GameTime gameTime)
    {
        EcsRuntime.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        EcsRuntime.Draw();
    }
}