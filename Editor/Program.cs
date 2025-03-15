using Launcher.UI;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Scenes;
using SharpEngine.Editor.Windows;

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
        var scene = args.Length > 0 ? Scene.LoadScene(args[0]) : new Scene();
        var project = args.Length > 0 ? Project.LoadProject(args[1]) : new Project() { Path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\Examples\Minimal"), Name = "Minimal" };

        using var window = new EditorWindow(scene, project, new DefaultViewSettings());
        window.OnLoaded += () => Console.WriteLine("test");

        window.Run();
    }
}