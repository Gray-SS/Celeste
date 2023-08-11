using Celeste.Editor.Rendering;
using ImGuiNET;

namespace Celeste.Editor.Windows;

public class PropertiesWindow : EditorWindow
{
    private object? _target;
    private IObjectVisualizer? _currentRenderer;

    private readonly List<IObjectVisualizer> _renderers;

    public PropertiesWindow(EditorRuntime runtime) : base(runtime)
    {
        _renderers = new List<IObjectVisualizer>();
        foreach (var type in typeof(EditorRuntime).Assembly.GetTypes().Where(x => !x.IsInterface && !x.IsAbstract && x.IsAssignableTo(typeof(IObjectVisualizer))))
        {
            var instance = (IObjectVisualizer)Activator.CreateInstance(type, runtime)!;
            _renderers.Add(instance);
        }
    }

    public void SetTarget(object target)
    {
        _target = target;
        _currentRenderer = _renderers.FirstOrDefault(x => x.RenderType == target.GetType());
    }

    public override void OnImGuiRender()
    {
        ImGui.Begin("Properties");

        if (_target != null)
        {
            _currentRenderer?.Render(_target);
        }

        ImGui.End();
    }
}
