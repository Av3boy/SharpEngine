using SharpEngine.Core;
using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Scenes;
using SharpEngine.Editor.Windows;
using SharpEngine.Shared;
using SharpEngine.Shared.Dto;
using System.Reflection;

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
        try
        {
            var editorPath = Environment.GetEnvironmentVariable(EnvironmentVariables.EDITOR_PATH_ENVIRONMENT_VARIABLE);
            if (string.IsNullOrWhiteSpace(editorPath))
                Environment.SetEnvironmentVariable(EnvironmentVariables.EDITOR_PATH_ENVIRONMENT_VARIABLE, System.Environment.ProcessPath, EnvironmentVariableTarget.User);

            string? sceneFile = null;
            string? projectFile = null;

            foreach (var arg in args)
            {
                if (arg.EndsWith(".sharpscene", StringComparison.OrdinalIgnoreCase))
                    sceneFile = arg;

                else if (arg.EndsWith(".sharpproject", StringComparison.OrdinalIgnoreCase))
                    projectFile = arg;
            }

            var scene = !string.IsNullOrEmpty(sceneFile) ? Scene.LoadScene(sceneFile) : new Scene();
            var project = !string.IsNullOrEmpty(projectFile) ? Project.LoadProject(projectFile)
#if DEBUG
                : new Project() { Path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\Examples\Minimal"), Name = "Minimal" };
#else
                : new Project();
#endif
            using var window = new EditorWindow(scene, project!, new DefaultViewSettings());
            window.OnLoaded += () => Console.WriteLine("test");

            window.Run();
        }
        catch (Exception e)
        {
            Debug.LogInformation($"Failed to start the editor: {e.Message}", e, true);
        }
    }
}