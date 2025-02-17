using Core;
using Core.Entities;
using Core.Interfaces;
using Core.Renderers;

using Silk.NET.Maths;
using Silk.NET.OpenGL.Extensions.ImGui;

public class Program
{
    private static ImGuiController _imGuiController;

    public static void Main(string[] args)
    {
        // TODO: Load scene as a command line argument.

        var editor = new Editor(new DefaultViewSettings() 
        { 
            WindowOptions = Silk.NET.Windowing.WindowOptions.Default with 
            { 
                Size = new Vector2D<int>(1280, 720) }
            }
        );
        var scene = new Scene();

        Window window = new Window(scene, editor.CoreSettings, editor);

        _imGuiController = new ImGuiController(Window.GL, window, window.Input);
    }
}

public class EditorSettings : IViewSettings
{
    /// <inheritdoc />
    public bool UseWireFrame { get; set; }

    /// <inheritdoc />
    public bool PrintFrameRate { get; set; }

    /// <inheritdoc />
    public RenderFlags RendererFlags { get; set; }

    /// <inheritdoc />
    public Silk.NET.Windowing.WindowOptions WindowOptions { get; set; } = Silk.NET.Windowing.WindowOptions.Default;

    /// <inheritdoc />
    public float Sensitivity { get; set; }
}

public class Editor : View
{
    public Editor(IViewSettings settings) : base(new EditorSettings())
    {
        CoreSettings = settings;
    }

    public ISettings CoreSettings { get; init; }

    public void SaveScene(Scene scene)
    {

    }

    public void LoadScene(string sceneFile)
    {
    }
}