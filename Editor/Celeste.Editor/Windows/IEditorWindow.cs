namespace Celeste.Editor.Windows;

public class VisibilityChanged
{
    public IEditorWindow Window { get; }

    public bool Old { get; }

    public bool New { get; }

    public VisibilityChanged(IEditorWindow window, bool old, bool @new)
    {
        Window = window;
        Old = old;
        New = @new;
    }
}

public interface IEditorWindow
{
    bool Visible { get; set; }

    event EventHandler<VisibilityChanged>? VisibilityChanged;

    void Show();

    void Hide();

    void SetVisibility(bool visible);

    void OnImGuiRender();
}
