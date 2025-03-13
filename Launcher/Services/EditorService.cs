using Launcher.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Launcher.Services
{
    public interface IEditorService
    {
        void Initialize(Project project);
        void OpenInEditor(Project project);
    }

    public class EditorService : IEditorService
    {
        private const string SHARP_ENGINE_PROJECT_EXTENSION = ".sharpproject";
        private const string SHARP_ENGINE_CORE_NUGET_PACKAGE = "SharpEngine.Core";
        private const string FRAMEWORK = "net8.0";

        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Initializes a new instance of <see cref="EditorService"/>.
        /// </summary>
        /// <param name="notificationService">The notification message service used to display messages in the UI.</param>
        public EditorService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <inheritdoc />
        public void Initialize(Project project)
        {
            CreateProjectDirectory(project);
            CreateSharpEngineProject(project);
            CreateSolution(project);
        }

        private static void CreateProjectDirectory(Project project)
        {
            string projectDirectory = Path.GetDirectoryName(Path.GetFullPath(project.Path!))!;
            Directory.CreateDirectory(projectDirectory);
        }

        private static void CreateSharpEngineProject(Project project)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(project);
            string projectFilePath = Path.Join(project.Path, $"{project.Name}.{SHARP_ENGINE_PROJECT_EXTENSION}");
            File.WriteAllText(projectFilePath, json);
        }

        private void CreateSolution(Project project)
        {
            string projectName = project.Name!.Replace(" ", "_");

            // Define individual commands
            string createSolution = $"dotnet new sln -n {projectName}";
            string createProject = $"dotnet new console -n {projectName} -o {project.Path} -f {FRAMEWORK}";
            string addProjectToSolution = $"dotnet sln {projectName}.sln add {project.Path}/{projectName}.csproj";

            // TODO: Add the nuget package after it's been published
            //string installNugetPackage = $"dotnet add {project.Path}/{projectName}.csproj package {SHARP_ENGINE_CORE_NUGET_PACKAGE}";
            string installNugetPackage = "echo done";

            // Determine the correct argument string based on platform
            string arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                 $"/c {createSolution} && {createProject} && {addProjectToSolution} && {installNugetPackage}" :
                $"-c \"{createSolution}; {createProject}; {addProjectToSolution}; {installNugetPackage}\"";

            // Setup process
            Process process = GetProcess(arguments);
            process.Start();
            process.WaitForExit();

            // Read and display output (optional)
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(error))
                _notificationService.Show(error);

            string programFilePath = Path.Combine(project.Path!, "Program.cs");
            string programContent = ""; // TODO: Replace with program content

            File.WriteAllText(programFilePath, programContent);
        }

        private static Process GetProcess(string arguments)
            => new()
            {

                StartInfo = new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/sh",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

        public void OpenInEditor(Project project)
        {
            if (!Path.Exists(project.Path))
            {
                _notificationService.Show($"The project file '{project.Path}' no longer exists.");
                return;
            }

            try
            {
                const string editorPathEnvironmentVariable = "SHARPENGINE_EDITOR_PATH";
                string? editorPath = Environment.GetEnvironmentVariable(editorPathEnvironmentVariable);
                if (editorPath is null)
                {
                    _notificationService.Show("Editor variable not found");
                    return;
                }

                const string editorExecutable = "SharpEngine.Editor.exe";

                string projectFile = project.Path.EndsWith(SHARP_ENGINE_PROJECT_EXTENSION) ?
                    project.Path : $"{project.Path}/{project.Name}.{SHARP_ENGINE_PROJECT_EXTENSION}";

                var process = GetProcess($"{editorPath}/{editorExecutable} {projectFile}");
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _notificationService.Show($"An error occurred while opening the project in the editor.", ex.Message);
            }
        }
    }
}
