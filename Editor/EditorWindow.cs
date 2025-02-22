using Core;
using Core.Interfaces;
using SharpEngine.Core.Scenes;
using View = Core.Entities.View;

using ImGuiNET;
using Silk.NET.Input;
using System.Numerics;
using Core.Primitives;
using Core.Entities;
using System.Reflection;
using SharpEngine.Core.Extensions;
using SharpEngine.Core.Attributes;

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
    /// <param name="view">The view for the window.</param>
    public EditorWindow(Scene scene, ISettings settings, View view) : base(scene, settings, view)
    {
    }

    private bool _showContextMenu;
    private bool _updateContextMenuLocation;

    /// <inheritdoc />
    protected override void AfterRender(double deltaTime)
    {
        base.AfterRender(deltaTime);

        RenderActionsMenu();
        RenderContextMenu();

        if (Scene.ActiveElement is not null)
            RenderActiveElementProperties(Scene.ActiveElement);
    }

    private void RenderActionsMenu()
    {
        ImGui.Begin("Window Actions");
        ImGui.Text("some text");

        if (ImGui.Button("Save"))
            SaveScene(Scene);

        if (ImGui.Button("Load") && SelectFile(out string selectedFile))
            LoadScene(selectedFile);

        ImGui.End();
    }

    private void RenderContextMenu()
    {
        if (_showContextMenu)
        {
            ImGui.Begin("Context menu");

            if (_updateContextMenuLocation)
            {
                ImGui.SetNextWindowPos(ImGui.GetMousePos());
                _updateContextMenuLocation = false;
            }

            if (ImGui.Button("Crete cube"))
            {
                var cube = PrimitiveFactory.Create(PrimitiveType.Cube, new Vector3(0, 0, 0));
                Scene.ActiveElement = cube;
                Scene.Root.AddChild(cube);
            }

            ImGui.End();
        }
    }

    public static void RenderActiveElementProperties(object obj, int maxRecursionDepth = 5, int currentDepth = 0)
    {
        if (currentDepth > maxRecursionDepth)
            return;

        if (currentDepth == 0)
            ImGui.Begin("Properties");

        try
        {
            var properties = obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var inspectorAttribute = property.GetCustomAttribute<InspectorAttribute>();
                if (inspectorAttribute != null && !inspectorAttribute.DisplayInInspector)
                {
                    continue;
                }

                var propertyType = property.PropertyType;
                var propertyName = inspectorAttribute?.DisplayName ?? property.Name;
                var propertyValue = property.GetValue(obj);

                if (propertyType == typeof(string))
                {
                    var value = propertyValue as string ?? string.Empty;
                    if (ImGui.InputText(propertyName, ref value, 100))
                    {
                        property.SetValue(obj, value);
                    }
                }
                else if (propertyType == typeof(int))
                {
                    var value = (int)(propertyValue ?? 0);
                    if (ImGui.SliderInt(propertyName, ref value, 0, 100))
                    {
                        property.SetValue(obj, value);
                    }
                }
                else if (propertyType == typeof(float))
                {
                    var value = (float)(propertyValue ?? 0f);
                    if (ImGui.SliderFloat(propertyName, ref value, 0f, 100f))
                    {
                        property.SetValue(obj, value);
                    }
                }
                else if (propertyType == typeof(Vector3))
                {
                    var value = (Vector3)(propertyValue ?? Vector3.Zero);
                    if (ImGui.SliderFloat3(propertyName, ref value, 0f, 100f))
                    {
                        property.SetValue(obj, value);
                    }
                }
                else if (propertyType.IsClass && propertyType != typeof(string))
                {
                    if (ImGui.CollapsingHeader(propertyName, ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.Indent();
                        RenderActiveElementProperties(propertyValue, maxRecursionDepth, currentDepth + 1);
                        ImGui.Unindent();
                    }
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