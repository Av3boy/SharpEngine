using Launcher.UI;
using SharpEngine.Shared;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Launcher.Services
{
    /// <summary>
    ///     Contains definitions for interacting with the editor.
    /// </summary>
    public interface IEditorService
    {
        /// <summary>
        ///     Creates a new project and solution.
        /// </summary>
        /// <param name="project">The project to be created.</param>
        void Initialize(Project project);

        /// <summary>
        ///     Opens the specified project in the editor.
        /// </summary>
        /// <param name="filePath">The file to be opened.</param>
        /// <returns>
        ///     A <see cref="Task" /> representing an asynchronous operation.
        ///     The result of the operation contains the project that was loaded; <see langword="null" /> if the project could not be loaded.
        /// </returns>
        Task<Project?> LoadFileAsync(string filePath);

        /// <summary>
        ///     Loads the projects previously associated with the launcher.
        /// </summary>
        /// <param name="projectsFile">The projects file.</param>
        /// <returns>
        ///     All the found projects.
        /// </returns>
        List<Project> LoadProjects(string projectsFile);

        /// <summary>
        ///     Opens the specified project in the editor.
        /// </summary>
        /// <param name="project"></param>
        void OpenInEditor(Project project);
    }

    /// <summary>
    ///     Handles interactions with the editor.
    /// </summary>
    public class EditorService : IEditorService
    {
        private const string SHARP_ENGINE_PROJECT_EXTENSION = "sharpproject";
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
            CreateSharpEngineProject(project);
            CreateSolution(project);
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
            string createSolution = $"dotnet new sln -n {projectName} -o {project.Path}";
            string createProject = $"dotnet new console -n {projectName} -o {project.Path} -f {FRAMEWORK}";
            string addProjectToSolution = $"dotnet sln {project.Path}/{projectName}.sln add {project.Path}/{projectName}.csproj";

            // TODO: Add the NuGet package after it's been published
            //string installNuGetPackage = $"dotnet add {project.Path}/{projectName}.csproj package {SHARP_ENGINE_CORE_NUGET_PACKAGE}";
            string installNuGetPackage = "echo done";

            // Determine the correct argument string based on platform
            string arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                 $"/c {createSolution} && {createProject} && {addProjectToSolution} && {installNuGetPackage}" :
                $"-c \"{createSolution}; {createProject}; {addProjectToSolution}; {installNuGetPackage}\"";

            // Setup process
            Process process = ProcessExtensions.GetProcess(arguments);
            process.Start();
            process.WaitForExit();

            // Read and display output
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(error))
                _notificationService.Show(error);
            else
                _notificationService.Show("Solution and project created successfully!");

            // TODO: Set example program.cs content with the minimal projects content.
            // string programFilePath = Path.Combine(project.Path!, "Program.cs");
            // string programContent = ""; // TODO: Replace with program content
            // 
            // File.WriteAllText(programFilePath, programContent);
        }

        /// <inheritdoc />
        public void OpenInEditor(Project project)
        {
            _notificationService.Show($"Starting {project.Name}..");

            if (!Path.Exists(project.Path))
            {
                _notificationService.Show($"The project file '{project.Path}' no longer exists.");
                return;
            }

            try
            {
                string? editorExecutableFilePath = Environment.GetEnvironmentVariable(EnvironmentVariables.EDITOR_PATH_ENVIRONMENT_VARIABLE);
                if (editorExecutableFilePath is null)
                {
                    _notificationService.Show("Editor variable not found");
                    return;
                }

                string projectFile = project.Path.EndsWith(SHARP_ENGINE_PROJECT_EXTENSION) ?
                    project.Path : $"{project.Path}\\{project.Name}.{SHARP_ENGINE_PROJECT_EXTENSION}";

                ProcessExtensions.RunProcess($"{editorExecutableFilePath} {projectFile}", true, msg => _notificationService.ShowAsync(msg));
            }
            catch (Exception ex)
            {
                _notificationService.Show($"An error occurred while opening the project in the editor.", false, ex.Message);
            }
        }

        /// <inheritdoc />
        public List<Project> LoadProjects(string projectsFile)
        {
            if (!File.Exists(projectsFile))
            {
                File.WriteAllText(projectsFile, "[]");
                return [];
            }

            string json = File.ReadAllText(projectsFile);
            var projects = System.Text.Json.JsonSerializer.Deserialize<List<Project>>(json);

            if (projects is not null)
            {
                // TOOD: Resolve last modified
                return projects;
            }

            _notificationService.Show($"Unable to load projects.", false, json, projects?.ToArray());
            return [];
        }

        /// <inheritdoc />
        public async Task<Project?> LoadFileAsync(string filePath)
        {
            if (!filePath.EndsWith(SHARP_ENGINE_PROJECT_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                _notificationService.Show($"The file '{filePath}' is not a valid project file.");
                return null;
            }

            using var reader = new StreamReader(filePath);
            string fileContent = await reader.ReadToEndAsync();

            return System.Text.Json.JsonSerializer.Deserialize<Project>(fileContent);
        }
    }
}
