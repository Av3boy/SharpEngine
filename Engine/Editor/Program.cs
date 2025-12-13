using SharpEngine.Core.Entities.Views.Settings;
using SharpEngine.Core.Scenes;

using SharpEngine.Shared;
using SharpEngine.Shared.Dto;

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
        Scene scene = new Scene();
        Project project =
        #if DEBUG
                new Project() { Path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\Examples\Minimal"), Name = "Minimal" };
#else
                new Project();
#endif

        static bool TryGetProject(string projectFile, out Project? project)
        {
            project = Project.LoadProject(projectFile);
            return project is not null;
        }

        try
        {
            var editorPath = Environment.GetEnvironmentVariable(EnvironmentVariables.EDITOR_PATH_ENVIRONMENT_VARIABLE);
            if (string.IsNullOrWhiteSpace(editorPath))
                Environment.SetEnvironmentVariable(EnvironmentVariables.EDITOR_PATH_ENVIRONMENT_VARIABLE, Environment.ProcessPath, EnvironmentVariableTarget.User);

            string? sceneFile = null;
            string? projectFile = null;

            foreach (var arg in args)
            {
                if (arg.EndsWith(".sharpscene", StringComparison.OrdinalIgnoreCase))
                    sceneFile = arg;

                else if (arg.EndsWith(".sharpproject", StringComparison.OrdinalIgnoreCase))
                    projectFile = arg;
            }

            if (!string.IsNullOrEmpty(sceneFile))
                scene = Scene.LoadScene(sceneFile);

            if (!string.IsNullOrEmpty(projectFile) && TryGetProject(projectFile, out Project? loadedProject))
                project = loadedProject!;
        }
        catch (Exception ex)
        {
            Debug.Log.Information(ex, "Failed to start the editor: {Message}", ex.Message);
        }

        try
        {
            using var window = new EditorWindow(scene, project!, new DefaultViewSettings());
            window.OnLoaded += () => Debug.Log.Information("test");

            window.Run();
        }
        catch (Exception ex)
        {
            Debug.Log.Information(ex, "Unexpected error while running editor: {Message}", ex.Message);
        }
    }
}