using ImGuiNET;
using Launcher.UI;
using SharpEngine.Core;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Primitives;
using SharpEngine.Core.Scenes;
using Silk.NET.Input;
using System.Numerics;
using System.Reflection;

namespace SharpEngine.Editor.Windows;

/// <summary>
///     Represents an editor window.
/// </summary>
public class EditorWindow : Window
{
    private Project _project = new Project() { Path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\Examples\Minimal"), Name = "Minimal" };
    private readonly List<ImGuiWindowBase> _windows = [];

    /// <summary>
    ///     Initializes an editor window with a specified scene and view settings.
    /// </summary>
    /// <param name="scene">Represents the current scene to be displayed in the editor window.</param>
    /// <param name="settings">Contains configuration options for how the view should be rendered.</param>
    public EditorWindow(Scene scene, IViewSettings settings) : base(scene, settings.WindowOptions) { }

    /// <inheritdoc />
    public override void OnLoad()
    {
        base.OnLoad();

        // Test only: remove later.
        var cube = PrimitiveFactory.Create(PrimitiveType.Cube, new Vector3(1, 1, 1));
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
                var window = Activator.CreateInstance(type, Scene, _project) ?? 
                    throw new InvalidOperationException($"Error initializing window: {type}");

                _windows.Add((ImGuiWindowBase)window);
            }
            catch (Exception e)
            {
                Debug.LogInformation(e.Message, e);
            }
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

        if (button == MouseButton.Right)
        {
            var contextMenuWindow = _windows.FirstOrDefault(w => w.GetType() == typeof(ContextMenuWindow));
            ((ContextMenuWindow?)contextMenuWindow)?.ShowContextMenu();
        }
    }
}
