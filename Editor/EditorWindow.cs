using SharpEngine.Core.Scenes;
using SharpEngine.Core.Attributes;

using ImGuiNET;
using Silk.NET.Input;
using System.Numerics;
using System.Reflection;

using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Primitives;
using SharpEngine.Core;

namespace SharpEngine.Editor;

/// <summary>
///     Represents an editor window.
/// </summary>
public class EditorWindow : Window
{
    /// <summary>
    ///     Initializes a new instance of <see cref="EditorWindow"/>.
    /// </summary>
    /// <param name="scene">The scene to be initialized.</param>
    /// <param name="settings">The settings for the editor window.</param>
    public EditorWindow(Scene scene, IViewSettings settings) : base(scene, settings.WindowOptions)
    {
    }

    private Project _project = new Project() { Path = @"C:\test" };

    private bool _showContextMenu;
    private bool _updateContextMenuLocation;

    /// <inheritdoc />
    public override void OnLoad()
    {
        base.OnLoad();

        // Test only: remove later.
        var cube = PrimitiveFactory.Create(PrimitiveType.Cube, new System.Numerics.Vector3(1, 1, 1));
        cube.Name = "test";

        Scene.Root.Children.Add(cube);
    }

    /// <inheritdoc />
    protected override void AfterRender(double deltaTime)
    {
        base.AfterRender(deltaTime);

        RenderActionsMenu();
        RenderContextMenu();
        RenderScene();
        RenderActiveElementProperties(Scene.ActiveElement);
        RenderAssets();
    }

    private void RenderAssets()
    {
        ImGui.Begin("Assets");

        string assetsPath = _project.Path;
        RenderDirectory(assetsPath);

        ImGui.End();
    }

    private void RenderDirectory(string path)
    {
        string[] directories = System.IO.Directory.GetDirectories(path);
        string[] files = System.IO.Directory.GetFiles(path);

        foreach (var directory in directories)
        {
            if (ImGui.TreeNode(System.IO.Path.GetFileName(directory)))
            {
                RenderDirectory(directory);
                ImGui.TreePop();
            }
        }

        foreach (var file in files)
        {
            if (ImGui.Selectable(System.IO.Path.GetFileName(file)))
            {
                // Scene.ActiveElement = file; // Set the file as the active element
            }
        }
    }

    private void RenderScene()
    {
        ImGui.Begin("Scene");

        if (Scene.Root.Children.Count == 0)
            ImGui.Text("No objects in the scene.");
        else
            RenderSceneNode(Scene.Root);

        ImGui.End();
    }

    private void RenderSceneNode(SceneNode node)
    {
        foreach (var child in node.Children.Where(child => ImGui.TreeNode(child.Name)))
        {
            Scene.ActiveElement = child;

            ImGui.Indent();
            RenderSceneNode(child);
            ImGui.Unindent();
            ImGui.TreePop();
        }
    }

    private void RenderActionsMenu()
    {
        ImGui.Begin("Window Actions");
        ImGui.Text("some text");

        if (ImGui.Button("Save"))
            SaveScene(Scene);

        if (ImGui.Button("Load") && SelectFile(out string selectedFile))
            LoadScene(selectedFile);

        if (ImGui.Button("Play"))
            StartGame();

        ImGui.End();
    }

    private void StartGame()
    {
        // TOOD: build project and start it.
    }

    private void RenderContextMenu()
    {
        if (_showContextMenu)
        {
            if (_updateContextMenuLocation)
            {
                ImGui.SetNextWindowPos(ImGui.GetMousePos());
                _updateContextMenuLocation = false;
            }

            ImGui.Begin("Context menu");

            if (ImGui.Button("Crete cube"))
            {
                var cube = PrimitiveFactory.Create(PrimitiveType.Cube, new Vector3(0, 0, 0));
                cube.Name = "Cube" + Scene.Root.Children.Count;

                Scene.ActiveElement = cube;
                Scene.Root.AddChild(cube);
            }

            ImGui.End();
        }
    }

    /// <summary>
    ///    Renders the properties of the active element.
    /// </summary>
    /// <param name="obj">The object whose properties should be rendered onto the window.</param>
    /// <param name="maxRecursionDepth">Marks how deep the inspector is allowed render properties.</param>
    /// <param name="currentDepth">Marks the starting level or the currently rendered level of properties.</param>
    public void RenderActiveElementProperties(object obj, int maxRecursionDepth = 5, int currentDepth = 0)
    {
        if (currentDepth == 0)
            ImGui.Begin(obj is null ? "Properties" : Scene.ActiveElement.Name + " properties");

        if (obj is null)
        {
            ImGui.Text("No properties to display.");
            ImGui.End();
            return;
        }

        if (currentDepth > maxRecursionDepth)
            return;

        try
        {
            var properties = obj.GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.PropertyType.IsClass && p.PropertyType != typeof(string));

            foreach (var property in properties)
            {
                var inspectorAttribute = property.GetCustomAttribute<InspectorAttribute>();
                if (inspectorAttribute != null && !inspectorAttribute.DisplayInInspector)
                {
                    continue;
                }

                var propertyName = inspectorAttribute?.DisplayName ?? property.Name;
                var propertyValue = property.GetValue(obj);

                object? newValue = propertyValue switch
                {
                    string stringValue => ImGui.InputText(propertyName, ref stringValue, 100) ? stringValue : propertyValue,
                    int intValue => ImGui.SliderInt(propertyName, ref intValue, 0, 100) ? intValue : propertyValue,
                    float floatValue => ImGui.SliderFloat(propertyName, ref floatValue, 0f, 100f) ? floatValue : propertyValue,
                    Vector3 vector3Value => ImGui.SliderFloat3(propertyName, ref vector3Value, 0f, 100f) ? vector3Value : propertyValue,
                    _ => propertyValue
                };

                if (newValue != propertyValue)
                    property.SetValue(obj, newValue);

                if (propertyValue is object && property.PropertyType.IsClass && property.PropertyType != typeof(string) &&
                    ImGui.CollapsingHeader(propertyName, ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Indent();
                    RenderActiveElementProperties(propertyValue, maxRecursionDepth, currentDepth + 1);
                    ImGui.Unindent();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        if (currentDepth == 0)
            ImGui.End();
    }

    /// <inheritdoc />
    public override void OnMouseClick(IMouse mouse, MouseButton button, Vector2 vector)
    {
        base.OnMouseClick(mouse, button, vector);

        if (button == MouseButton.Right)
        {
            _updateContextMenuLocation = true;
            _showContextMenu = true;
        }
    }

    private static void SaveScene(Scene scene)
    {
        Console.WriteLine("Saving..");

        if (!scene.HasUnsavedChanges && !scene.HasSaveFile())
        {
            Console.WriteLine("No changes to save.");
            return;
        }

        using var dialog = new SaveFileDialog()
        {
            Filter = $"Scene files (*.{Scene.SceneFileExtension})|*.{Scene.SceneFileExtension}",
            CheckWriteAccess = true,
            DefaultExt = Scene.SceneFileExtension,
            FileName = $"scene.{Scene.SceneFileExtension}",
            Title = "Save scene",
        };

        var dialogResult = dialog.ShowDialog();
        if (dialogResult == DialogResult.OK)
        {
            scene.SaveScene(dialog.FileName).Wait();
            Console.WriteLine("File saved successfully!");
        }
        else
            Console.WriteLine("File save cancelled.");
    }

    private static bool SelectFile(out string selectedFile)
    {
        Console.WriteLine("Selecting file..");

        using var dialog = new OpenFileDialog()
        {
            Title = "Load scene",
            Filter = $"Scene files (*.{Scene.SceneFileExtension})|*.{Scene.SceneFileExtension}",
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            selectedFile = dialog.FileName;
            return true;
        }
        else
        {
            selectedFile = string.Empty;
            return false;
        }
    }

    private void LoadScene(string sceneFile)
    {
        Console.WriteLine($"Loading scene from file: {sceneFile}.");

        var newScene = Scene.LoadScene(sceneFile);
        SetScene(newScene);

        Console.WriteLine("Scene loaded successfully!");
    }
}