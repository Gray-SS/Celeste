namespace Celeste.Editor.Rendering;

public abstract class ObjectVisualizer<T>
    : IObjectVisualizer
{
    public Type RenderType => typeof(T);

    public EditorRuntime Editor { get; }

    public ObjectVisualizer(EditorRuntime editor)
    {
        Editor = editor;
    }

    protected abstract void OnImGuiRender(T target);

    void IObjectVisualizer.Render(object target)
        => OnImGuiRender((T)target);
}
