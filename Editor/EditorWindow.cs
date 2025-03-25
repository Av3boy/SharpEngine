using ImGuiNET;
using Launcher.UI;

using SharpEngine.Core;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Primitives;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Entities;

using SharpEngine.Editor.Windows;

using Silk.NET.Input;

using System.Numerics;
using System.Reflection;

namespace SharpEngine.Editor;

/// <summary>
///     Represents an editor window.
/// </summary>
public class EditorWindow : Window
{
    private readonly Project _project;
    private readonly List<ImGuiWindowBase> _windows = [];

    private ContextMenuWindow? _contextMenuWindow;
    private ActionsMenuWindow? _actionsMenuWindow;

    /// <summary>
    ///     Initializes an editor window with a specified scene and view settings.
    /// </summary>
    /// <param name="scene">Represents the current scene to be displayed in the editor window.</param>
    /// <param name="project">The project loaded to the editor.</param>
    /// <param name="settings">Contains configuration options for how the view should be rendered.</param>
    public EditorWindow(Scene scene, Project project, IViewSettings settings) : base(scene, settings) 
    {
        _project = project;
    }

    /// <inheritdoc />
    public override void OnLoad()
    {
        base.OnLoad();

        try
        {
            // Test only: remove later.
            var cube = PrimitiveFactory.Create(PrimitiveType.Cube, new Vector3(3, 1, 1));
            cube.Name = "test";

            Scene.Root.Children.Add(cube);

            // Get all types that inherit from ImGuiWindowBase
            var windowTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ImGuiWindowBase)) && !t.IsAbstract)
                .ToArray();

            // Initialize all ImGuiWindowBase implementations
            for (int i = 0; i < windowTypes.Length; i++)
            {
                try
                {
                    var type = windowTypes[i];
                    var window = Activator.CreateInstance(type) ?? 
                        throw new InvalidOperationException($"Error initializing window: {type}");

                    var windowBase = (ImGuiWindowBase)window;
                    windowBase.SetScene(Scene);
                    windowBase.SetProject(_project);

                    _windows.Add(windowBase);
                }
                catch (Exception e)
                {
                    Debug.LogInformation(e.Message, e);
                }
            }

            _contextMenuWindow = (ContextMenuWindow?)_windows.FirstOrDefault(w => w.GetType() == typeof(ContextMenuWindow));
            if (_contextMenuWindow is null)
                Debug.LogInformation("ContextMenuWindow not found.");

            _actionsMenuWindow = (ActionsMenuWindow?)_windows.FirstOrDefault(w => w.GetType() == typeof(ActionsMenuWindow));
            if (_actionsMenuWindow is null)
                Debug.LogInformation("ActionsMenuWindow not found.");
            else
                _actionsMenuWindow.OnSceneLoaded += SetScene;
        }
        catch (Exception e)
        {
            Debug.LogInformation(e.Message, e);
        }
    }

    /// <inheritdoc />
    protected override void AfterRender(double deltaTime)
    {
        base.AfterRender(deltaTime);

        // Enable docking
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        // Create a docking space
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.Pos);
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowViewport(viewport.ID);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        ImGui.Begin("DockSpace", ImGuiWindowFlags.NoTitleBar |
                                     ImGuiWindowFlags.NoCollapse |
                                     ImGuiWindowFlags.NoResize |
                                     ImGuiWindowFlags.NoMove |
                                     ImGuiWindowFlags.NoBringToFrontOnFocus |
                                     ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.PopStyleVar(3);

        var dockSpaceId = ImGui.GetID("DockSpace");
        ImGui.DockSpace(dockSpaceId, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);

        ImGui.End();

        foreach (var window in _windows)
            window.RenderWindow();
    }

    /// <inheritdoc />
    public override void OnMouseClick(IMouse mouse, MouseButton button, Vector2 vector)
    {
        base.OnMouseClick(mouse, button, vector);

        if (button == Settings.PrimaryButton)
        {
            if (ImGui.IsAnyItemHovered())
            {
                // TODO: Right-clicked on an ImGui component
            }
            else
            {
                // TODO: Right-clicked outside of ImGui components
                Camera.IsInView(Scene, out GameObject? intersectingObject, out Vector3 hitPosition);
                _contextMenuWindow?.ShowContextMenu(intersectingObject);
            }
        }

        if (button == Settings.SecondaryButton)
        {
            if (ImGui.IsAnyItemHovered())
            {
                // TODO: Left-clicked on an ImGui component
            }
            else
            {
                Camera.IsInView(Scene, out GameObject? intersectingObject, out Vector3 hitPosition);
                Scene.ActiveElement = intersectingObject;
            }
        }
    }
}
