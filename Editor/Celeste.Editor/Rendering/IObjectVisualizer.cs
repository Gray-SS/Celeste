namespace Celeste.Editor.Rendering;

public interface IObjectVisualizer
{
    Type RenderType { get; }

    EditorRuntime Editor { get; }

    void Render(object target);
}
