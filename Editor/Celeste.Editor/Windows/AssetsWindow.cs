using ImGuiNET;
using Celeste.ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Editor.Rendering;
using Celeste.Common;

namespace Celeste.Editor.Windows;

public enum AssetKind
{
    Texture,
    Folder,
    Unknown
}

public class AssetItem
{
    public AssetKind Kind { get; }
    public string Name { get; }
    public string RelativePath { get; }
    public FileInfo? FileInfo { get; }

    public AssetItem(AssetKind kind, FileInfo? fileInfo, string relativePath, string fileName)
    {
        Kind = kind;
        Name = fileName;
        FileInfo = fileInfo;
        RelativePath = relativePath;
    }
}

public class AssetItemVisualizer : ObjectVisualizer<AssetItem>
{
    public AssetItemVisualizer(EditorRuntime editor) : base(editor)
    {
    }

    protected override void OnImGuiRender(AssetItem target)
    {
        if (target.Kind == AssetKind.Texture)
        {
            var texture = Assets.LoadTexture(target.RelativePath);
            var texturePtr = Editor.ImGuiManager.GetOrCreateTexturePtr(texture);
            ImGui.BeginChildFrame((uint)target.GetHashCode(), new ImVec2(25f, 25f), ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            ImGui.Image(texturePtr, new ImVec2(20f, 20f));
            ImGui.EndChildFrame();

            ImGui.SameLine();

            var textureName = Path.GetFileNameWithoutExtension(target.Name);
            ImGui.TextWrapped($"{textureName} (Texture 2D)");

            ImGui.Dummy(new ImVec2(0, 10));

            ImGui.Separator();

            ImGui.Dummy(new ImVec2(0, 10));

            ImGui.Text($"Name: {target.FileInfo!.Name}");
            ImGui.Text($"Width: {texture.Width}");
            ImGui.Text($"Height: {texture.Height}");
            ImGui.Text($"Size: {target.FileInfo!.Length} bytes");

            ImGui.Dummy(new ImVec2(0, 10));
            ImGui.Separator();
        }
    }
}

public class AssetsWindow : EditorWindow
{
    public static readonly string AssetsPath = AppDomain.CurrentDomain.BaseDirectory + "Assets";

    private float _itemWidth = 65f;
    private string _currentPath = string.Empty;

    private readonly List<AssetItem> _assets = new();
    private readonly Dictionary<AssetKind, Texture2D> _texturesLookup = new();

    public AssetsWindow(EditorRuntime runtime) : base(runtime)
    {
        _texturesLookup.Add(AssetKind.Texture, Resources.LoadTexture("file-icon.png"));
        _texturesLookup.Add(AssetKind.Unknown, Resources.LoadTexture("file-icon.png"));
        _texturesLookup.Add(AssetKind.Folder, Resources.LoadTexture("dir-icon.png"));

        this.NavigateTo(AssetsPath);
    }

    public void NavigateTo(string path)
    {
        _currentPath = path;
        RebuildAssets();
    }

    private void RebuildAssets()
    {
        _assets.Clear();

        var directories = Directory.GetDirectories(_currentPath);
        foreach (var dirPath in directories)
        {
            var dirRelativePath = Path.GetRelativePath(AssetsPath, dirPath);
            var assetItem = new AssetItem(AssetKind.Folder, null, dirRelativePath, Path.GetFileName(dirPath)!);
            _assets.Add(assetItem);
        }

        var files = Directory.GetFiles(_currentPath);
        foreach (var filePath in files)
        {
            var fileRelativePath = Path.GetRelativePath(AssetsPath, filePath);
            var fileExtension = Path.GetExtension(filePath);

            var assetType = fileExtension switch
            {
                ".jpeg" or ".jpg" or ".png" => AssetKind.Texture,
                _ => AssetKind.Unknown,
            };

            var assetItem = new AssetItem(assetType, new FileInfo(filePath), fileRelativePath, Path.GetFileName(filePath));
            _assets.Add(assetItem);
        }
    }

    public override void OnImGuiRender()
    {
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Color(15, 15, 15).ToNumerics());
        ImGui.Begin("Assets");

        var padding = 10;
        var panelWidth = ImGui.GetContentRegionAvail().X;
        var columnsCount = (int)(panelWidth / (_itemWidth + padding));

        ImGui.Columns(columnsCount, string.Empty, false);

        if (Directory.Exists(_currentPath))
        {
            if (_currentPath != AssetsPath && ImGui.ArrowButton("backBtn", ImGuiDir.Left))
                NavigateTo(Directory.GetParent(_currentPath)!.FullName);

            var newPath = _currentPath;

            foreach(var asset in _assets)
            {
                ImGui.PushID(asset.Name);

                ImGui.PushStyleColor(ImGuiCol.Button, new Color(20, 20, 20).ToNumerics());
                ImGui.ImageButton("dir-icon", Runtime.ImGuiManager.GetOrCreateTexturePtr(_texturesLookup[asset.Kind]), new Vector2(_itemWidth).ToNumerics());
                ImGui.PopStyleColor();

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                {
                    switch (asset.Kind)
                    {
                        case AssetKind.Folder:
                            newPath = Path.Combine(AssetsPath, asset.RelativePath);
                            break;
                        case AssetKind.Texture:
                            var window = Runtime.GetWindow<PropertiesWindow>();
                            window.SetTarget(asset);
                            break;
                    }
                }

                ImGui.TextWrapped(asset.Name);

                ImGui.PopID();

                ImGui.NextColumn();
            }

            if (newPath != _currentPath)
            {
                NavigateTo(newPath);
            }
        }

        ImGui.End();
    }
}
