using Core;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Renderers;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;

public class Program
{
    private static ImGuiController _imGuiController;

    public static void Main(string[] args)
    {
        // TODO: Load scene as a command line argument.

        var editor = new Editor();
        var scene = new Scene();

        Window window = new Window(editor, scene, Silk.NET.Windowing.WindowOptions.Default with
        {
            Title = "SharpEngine"
        });

        _imGuiController = new ImGuiController(Window.GL, window, window.Input);

    }
}

public class EditorSettings : ISettings
{
    public bool UseWireFrame { get; set; }
    public bool PrintFrameRate { get; set; }
    public RenderFlags RendererFlags { get; set; }
    public Silk.NET.Windowing.WindowOptions WindowOptions { get; set; }
}

public class Editor : IGame
{
    public ISettings CoreSettings { get; } = new EditorSettings();

    public Camera Camera { get; set; }

    public void HandleKeyboard(IKeyboard input, double deltaTime)
    {

    }
    public void HandleMouse(IMouse mouse)
    {

    }
    public void HandleMouseDown(IMouse mouse, MouseButton button)
    {

    }
    public void HandleMouseWheel(MouseWheelScrollDirection direction, ScrollWheel scrollWheel)
    {

    }
    public void Initialize()
    {

    }

    public void Update(double deltaTime, IInputContext input)
    {

    }

    public void SaveScene(Scene scene)
    {

    }

    public void LoadScene(string sceneFile)
    {
    }
}