namespace Celeste.Editor.Windows;

public abstract class EditorWindow
    : IEditorWindow
{
    public bool Visible
    {
        get => _visible;
        set => SetVisibility(value);
    }

    protected EditorRuntime Runtime { get; }

    private bool _visible = true;

    public event EventHandler<VisibilityChanged>? VisibilityChanged;

    public EditorWindow(EditorRuntime runtime)
        => Runtime = runtime;

    public void Show()
        => SetVisibility(true);

    public void Hide()
        => SetVisibility(false);

    public void SetVisibility(bool value)
    {
        if (value == _visible) return;

        var old = _visible;
        _visible = value;
        VisibilityChanged?.Invoke(this, new(this, old, value));
    }

    public abstract void OnImGuiRender();
}
