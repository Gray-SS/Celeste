using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Common;
using Celeste.ECS;
using Celeste.Common.Sprites;
using Celeste.ImGuiNET;

namespace Celeste.Editor;

public class EditorGameLogic : GameLogic
{
    public EditorRuntime Editor { get; }

    private ImGuiManager _gui = null!;
    private RenderTarget2D _renderTarget = null!;

    internal static EditorGameLogic Instance { get; private set; } = null!;

    public EditorGameLogic()
    {
        Instance = this;
        IsMouseVisible = true;

        Editor = new EditorRuntime(this);
    }

    protected override void Initialize()
    {
        base.Initialize();

        var cameraEntity = EcsRuntime.PerformQuery<Camera>().Single();
        Editor.BindEntity("Camera", cameraEntity);

        _gui = new ImGuiManager(this);
        _renderTarget = new RenderTarget2D(GraphicsDevice, 1280, 720);

        var io = ImGui.GetIO();
        io.ConfigFlags = ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.NavEnableKeyboard;

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

        //Render the GUI
        _gui.BeginDraw(gameTime);

        BeginDockspace();

        ShowGameWindow();
        ShowAssetsWindow();
        ShowInspectorWindow();
        Editor.DrawPropertiesWindow();

        EndDockspace();

        _gui.EndDraw();
    }

    private int selected = -1;

    private void ShowInspectorWindow()
    {
        ImGui.Begin("Inspector");

        var canShowCreatePopup = true;

        var count = 0;
        foreach (var entity in Editor.Entities)
        {
            if (ImGui.Selectable(entity.Name, selected == count))
            {
                selected = count;
                Editor.EntitySelection.Clear();
                Editor.EntitySelection.Add(entity);
            }

            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.MenuItem("Delete"))
                {
                    Editor.DestroyEntity(entity);
                }

                canShowCreatePopup = false;

                ImGui.EndPopup();
            }

            count++;
        }

        if (canShowCreatePopup && ImGui.BeginPopupContextWindow())
        {
            if (ImGui.BeginMenu("Create"))
            {
                if (ImGui.MenuItem("Empty")) Editor.CreateEntity("Empty");
            }

            ImGui.EndPopup();
        }

        ImGui.End();

        Editor.ApplyChanges();
    }

    private void ShowAssetsWindow()
    {
        ImGui.Begin("Assets");

        ImGui.End();
    }

    private void ShowGameWindow()
    {
        ImGui.Begin("Game", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

        var winSize = GetLargestSizeForViewport();

        var ptr = _gui.GetOrCreateTexturePtr(_renderTarget);
        var uv0 = new Vector2(0f, 0f).ToNumerics();
        var uv1 = new Vector2(1f, 1f).ToNumerics();
        ImGui.Image(ptr, winSize.ToNumerics(), uv0, uv1);

        ImGui.End();
    }

    private Vector2 GetLargestSizeForViewport()
    {
        var windowSize = ImGui.GetContentRegionAvail().ToXna();
        windowSize.X -= ImGui.GetScrollX();
        windowSize.Y -= ImGui.GetScrollY();

        var aspectRatio = DeviceManager.PreferredBackBufferWidth / (float)DeviceManager.PreferredBackBufferHeight;
        var aspectWidth = windowSize.X;
        var aspectHeight = aspectWidth / aspectRatio;

        if (aspectHeight > windowSize.Y)
        {
            aspectHeight = windowSize.Y;
            aspectWidth = aspectHeight * aspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
    }

    private void BeginDockspace()
    {
        var windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;

        var viewport = GraphicsDevice.Viewport;

        ImGui.SetNextWindowPos(new Vector2(0.0f, 0.0f).ToNumerics(), ImGuiCond.Always);
        ImGui.SetNextWindowSize(new Vector2(viewport.Width, viewport.Height).ToNumerics());
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoNavFocus
            | ImGuiWindowFlags.NoBringToFrontOnFocus;

        var open = true;
        ImGui.Begin("Dockspace demo", ref open, windowFlags);

        ImGui.PopStyleVar(2);

        ImGui.DockSpace(ImGui.GetID("Dockspace"));
    }

    private static void EndDockspace()
        => ImGui.End();
}
