using ImGuiNET;

using SharpEngine.Core;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Primitives;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Entities;

using SharpEngine.Editor.Windows;

using Silk.NET.Input;

using System.Numerics;
using System.Reflection;
using SharpEngine.Core.Windowing;
using SharpEngine.Shared.Dto;

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

    private bool showUnsavedChangesDialog = true;

    /// <inheritdoc />
    protected override void AfterRender(Frame frame)
    {
        base.AfterRender(frame);

        EnableDocking();
        RenderMenuBar();

        ImGui.Begin("Unsaved changes", ref showUnsavedChangesDialog, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove);
        ImGui.Text("There are unsaved changes in the project. Are you sure you want to exist without saving?");
        
        bool cancelled = false;
        if (ImGui.Button("Cancel"))
            cancelled = true;

        if (!cancelled && ImGui.Button("Save & close"))
        {
            Save();
            CurrentWindow.Close();
        }

        if (!cancelled && ImGui.Button("Exit without saving"))
            CurrentWindow.Close();

        ImGui.End();

        foreach (var window in _windows)
            window.RenderWindow();
    }

    private void Save()
    {
        // TODO: #84 Save all changes.
    }

    private static void EnableDocking()
    {
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
    }

    private void RenderMenuBar()
    {
        // Add a main menu bar
        if (ImGui.BeginMainMenuBar())
        {
            RenderFileMenu();

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Settings"))
                {
                    // Handle New action

                    // TODO: #76 Select preferred editor
                    // Required fields:
                    // radio button, each defined editor (store as json)
                    // textbox,Path to executable
                    // textbox, cli argument to open a file
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Windows"))
            {
                foreach (var window in _windows)
                {
                    if (ImGui.MenuItem(window.Name))
                        window.Open();
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

    private void RenderFileMenu()
    {
        if (ImGui.BeginMenu("File"))
        {
            if (ImGui.MenuItem("New"))
            {
                // TODO: #83 Handle New action
            }

            if (ImGui.MenuItem("Open"))
            {
                // TODO: #82 Handle Open action
            }

            if (ImGui.MenuItem("Save"))
                Save();

            if (ImGui.MenuItem("Exit"))
            {
                // TODO: #81 Handle Exit action
            }

            if (ImGui.MenuItem("Publish"))
            {
                // TODO: #80 Handle Publish action
            }

            ImGui.EndMenu();
        }
    }

    /// <inheritdoc />
    public override void OnMouseClick(IMouse mouse, MouseButton button, Vector2 vector)
    {
        base.OnMouseClick(mouse, button, vector);

        if (button == Settings.PrimaryButton)
        {
            // TODO: #79 This probably isn't doable using ImGui. Figure out a new way or wait until UI is written using the Core 2D renderer.
            if (ImGui.IsItemClicked((ImGuiMouseButton)button))
            {
                // Clicked on an ImGui component
                uint hoveredItemId = ImGui.GetID("");
                Debug.LogInformation($"Clicked on ImGui component with ID: {hoveredItemId}");
            }
            else
            {
                Camera.IsInView(Scene, out GameObject? intersectingObject, out Vector3 hitPosition);
                _contextMenuWindow?.ShowContextMenu(intersectingObject);
            }
        }

        if (button == Settings.SecondaryButton)
        {
            // TODO: #79 Make this work.
            if (ImGui.IsItemClicked((ImGuiMouseButton)button))
            {
                // Clicked on an ImGui component
                uint hoveredItemId = ImGui.GetID("");
                Debug.LogInformation($"Clicked on ImGui component with ID: {hoveredItemId}");
            }
            else
            {
                Camera.IsInView(Scene, out GameObject? intersectingObject, out Vector3 hitPosition);
                Scene.ActiveElement = intersectingObject;
            }
        }
    }
}
