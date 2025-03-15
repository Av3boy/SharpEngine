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
        //var editor = new Editor();
        var scene = args.Length > 0 ? Scene.LoadScene(args[0]) : new Scene();

        using var window = new EditorWindow(scene, new DefaultViewSettings());
        window.OnLoaded += () => Console.WriteLine("test");

        window.Run();
    }
}