using Launcher.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Launcher.Services
{
    public interface IProjectInitializationService
    {
        void Initialize(Project project);
    }

    public class ProjectInitializationService : IProjectInitializationService
    {
        private const string SHARP_ENGINE_CORE_NUGET_PACKAGE = "SharpEngine.Core";
        private const string FRAMEWORK = "net8.0";

        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Initializes a new instance of <see cref="ProjectInitializationService"/>.
        /// </summary>
        /// <param name="notificationService">The notification message service used to display messages in the UI.</param>
        public ProjectInitializationService(INotificationService notificationService)
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
            File.WriteAllText(Path.Join(project.Path, $"{project.Name}.sharpproject"), json);
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
            var process = new Process
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
    }
}
