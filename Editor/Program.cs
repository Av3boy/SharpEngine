using Core.Entities;
using SharpEngine.Core.Scenes;
using View = Core.Entities.View;
using Silk.NET.Maths;

namespace SharpEngine.Editor;

/// <summary>
///    Represents the main entry point of the application.
/// </summary>
public static class Program
{
    /// <summary>
    ///    The main entry point of the application.
    /// </summary>
    /// <param name="args">The arguments passed with the start call.</param>
    [STAThread]
    public static void Main(string[] args)
    {
        var editor = new View(new DefaultViewSettings() 
        { 
            WindowOptions = Silk.NET.Windowing.WindowOptions.Default with 
            { 
                Size = new Vector2D<int>(1280, 720) }
            }
        );

        var scene = args.Length > 0 ? Scene.LoadScene(args[0]) : new Scene();
        using var window = new EditorWindow(scene, editor.Settings, editor);
    }
}