using Celeste.ECS;
using Celeste.Common;
using Celeste.Editor.Windows;
using Celeste.ImGuiNET;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Editor.Rendering;

namespace Celeste.Editor;

public class EditorRuntime
{
    public IReadOnlyList<DomainEntity> Entities => _entities;
    public ImGuiManager ImGuiManager => _gui;

    private readonly GameLogic _game;
    private readonly List<IEditorWindow> _windows;
    private readonly List<DomainEntity> _entities;
    private readonly List<PropertyDrawer> _propertyDrawers;
    private readonly Queue<DomainEntity> _toDestroyEntities = new();

    private readonly ImGuiManager _gui;

    public EditorRuntime(GameLogic game)
    {
        _game = game;
        _gui = new ImGuiManager(game);
        _entities = new List<DomainEntity>();

        _windows = new List<IEditorWindow>();
        foreach (var type in typeof(EditorRuntime).Assembly.GetTypes().Where(x => !x.IsInterface && !x.IsAbstract && x.IsAssignableTo(typeof(EditorWindow))))
        {
            var instance = (EditorWindow)Activator.CreateInstance(type, this)!;
            _windows.Add(instance);
        }

        _propertyDrawers = new List<PropertyDrawer>();
        foreach (var type in typeof(EditorRuntime).Assembly.GetTypes().Where(x => !x.IsInterface && !x.IsAbstract && x.IsAssignableTo(typeof(PropertyDrawer))))
        {
            var instance = (PropertyDrawer)Activator.CreateInstance(type)!;
            _propertyDrawers.Add(instance);
        }
    }

    public PropertyDrawer? GetPropertyDrawer(Type type)
    {
        var propDrawer = _propertyDrawers.FirstOrDefault(x => x.PropertyType == type);
        return propDrawer;
    }

    public T GetWindow<T>() where T : IEditorWindow
        => (T)GetWindow(typeof(T));

    public IEditorWindow GetWindow(Type windowType)
    {
        if (!windowType.IsAssignableTo(typeof(IEditorWindow)))
            throw new InvalidOperationException("Invalid type inputed");

        var window = _windows.FirstOrDefault(x => x.GetType() == windowType)
            ?? throw new InvalidOperationException($"Window with type {windowType.Name} doesn't exists");

        return window;
    }

    public DomainEntity BindEntity(string name, Entity entity)
    {
        var domainEntity = new DomainEntity(name, entity, _game.EcsRuntime);
        _entities.Add(domainEntity);

        return domainEntity;
    }

    public void DestroyEntity(DomainEntity entity)
        => _toDestroyEntities.Enqueue(entity);

    public DomainEntity CreateEntity(string name)
    {
        var entity = _game.EcsRuntime.CreateEntity();
        var domainEntity = new DomainEntity(name, entity, _game.EcsRuntime);
        _entities.Add(domainEntity);

        return domainEntity;
    }

    public void ApplyChanges()
    {
        while (_toDestroyEntities.Count > 0)
        {
            var entity = _toDestroyEntities.Dequeue();
            _game.EcsRuntime.DestroyEntity(entity.Entity);
            _entities.Remove(entity);
        }
    }

    public void OnImGuiRender(RenderTarget2D gameTexture, GameTime gameTime)
    {
        _gui.BeginDraw(gameTime);

        BeginDockspace();

        ShowGameWindow(gameTexture);

        foreach(var window in _windows)
        {
            if (!window.Visible)
                continue;

            window.OnImGuiRender();
        }

        EndDockspace();

        _gui.EndDraw();
    }

    private Vector2 GetLargestSizeForViewport()
    {
        var windowSize = ImGui.GetContentRegionAvail().ToXna();
        windowSize.X -= ImGui.GetScrollX();
        windowSize.Y -= ImGui.GetScrollY();

        var aspectRatio = _game.DeviceManager.PreferredBackBufferWidth / (float)_game.DeviceManager.PreferredBackBufferHeight;
        var aspectWidth = windowSize.X;
        var aspectHeight = aspectWidth / aspectRatio;

        if (aspectHeight > windowSize.Y)
        {
            aspectHeight = windowSize.Y;
            aspectWidth = aspectHeight * aspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
    }

    private void ShowGameWindow(RenderTarget2D gameTexture)
    {
        ImGui.Begin("Game", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

        var winSize = GetLargestSizeForViewport();

        var ptr = _gui.GetOrCreateTexturePtr(gameTexture);
        var uv0 = new Vector2(0f, 0f).ToNumerics();
        var uv1 = new Vector2(1f, 1f).ToNumerics();
        ImGui.Image(ptr, winSize.ToNumerics(), uv0, uv1);

        ImGui.End();
    }

    private void BeginDockspace()
    {
        var windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;

        var viewport = _game.GraphicsDevice.Viewport;

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