using Launcher.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Launcher.UI
{
    public class Message
    {
        public readonly Guid Id = Guid.NewGuid();
        public string Contents { get; set; } = string.Empty;

        public Message(string contents)
        {
            Contents = contents;
        }
    }

    /// <summary>Represents the home page of the launcher.</summary>
    public partial class Home : ComponentBase
    {
        [Inject]
        private IProjectInitializationService _projectInitializationService { get; init; } = default!;

        [Inject]
        private INotificationService _notificationService { get; init; } = default!;

        [Inject]
        private IApplicationManager _applicationManager { get; init; } = default!;

        private bool _showNotification;
        private bool _showConfirmDelete;
        private bool _showCreateProjectDialog;

        // TODO: The notification messages should be added into a separate service.
        private Dictionary<Message, int> _notificationMessages = new()
        {
            { new("some message"), 2 },
            { new("another"), 2 },
        };

        private const string _projectsFile = "projects.json";
        private const string _launcherEnvironmentVariable = "SHARP_ENGINE_LAUNCHER_DIRECTORY";
        private readonly string _currentDirectory = AppContext.BaseDirectory;
        private string _projectDirectory => Path.Join(_currentDirectory, _projectsFile);


        private List<Project> Projects { get; set; } = new()
        {
            // TODO: Remove these mock projects once the actual projects are loaded.
            new Project { Name = "Project 1", LastModified = DateTime.Now },
            new Project { Name = "Project 2", LastModified = DateTime.Now.AddDays(6).AddHours(2) },
        };

        private Project _selectedProject = new();

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // TODO: Get current launcher version

            // TODO: Check if GitHub has a new version released.

            CheckEnvironmentVariable();
            // LoadProjects();
        }

        private void CheckEnvironmentVariable()
        {
            var launcherDirectory = Environment.GetEnvironmentVariable(_launcherEnvironmentVariable);
            if (string.IsNullOrEmpty(launcherDirectory) || launcherDirectory != _currentDirectory)
                Environment.SetEnvironmentVariable(_launcherEnvironmentVariable, _currentDirectory);
        }

        private void LoadProjects()
        {
            if (!File.Exists(_projectDirectory))
                File.WriteAllText(_projectDirectory, "[]");

            string json = File.ReadAllText(_projectDirectory);
            var projects = System.Text.Json.JsonSerializer.Deserialize<List<Project>>(json);

            if (projects is not null)
                Projects = projects;
            else
                HandleMessage($"Unable to load projects.", json, projects);
        }

        private async Task LoadFileAsync(InputFileChangeEventArgs e)
        {
            // TODO: Check if file is a "sharpproject.json" file

            using var stream = e.File.OpenReadStream();
            using var reader = new StreamReader(stream);
            string fileContent = await reader.ReadToEndAsync();

            var project = System.Text.Json.JsonSerializer.Deserialize<Project>(fileContent);

            if (project is not null)
                Projects.Add(project);
            else
                HandleMessage($"Unable to load project file.", project, e.File.Name, fileContent);
        }

        private void OnCreateProjectClicked()
            => _showCreateProjectDialog = true;

        private void OnDeleteProjectClicked(Project project)
        {
            _selectedProject = project;
            _showConfirmDelete = true;
        }

        private void OnDeleteProjectConfirm()
        {
            var project = Projects.Find(p => p.Id == _selectedProject.Id);

            if (project is not null)
                Projects.Remove(project);
            else
                HandleMessage($"Unable to delete project '{project?.Name}'.", _selectedProject, Projects);
        }

        private void ProjectCreated(Project project)
        {
            var projectDirectory = Path.GetDirectoryName(Path.GetFullPath(project.Path));
            Directory.CreateDirectory(projectDirectory);

            var json = System.Text.Json.JsonSerializer.Serialize(project);
            File.WriteAllText(Path.Join(projectDirectory, $"{project.Name}.sharpproject"), json);

            // TODO: Create visual studio project containing a reference to the Core project.

            Projects.Add(project);
        }

        private void HandleMessage(string message, params object?[] details)
        {
            // TODO: Get the message using the Id
            // if (!_notificationMessages.TryGetValue(message, out int amount))
                 _notificationMessages.Add(new Message(message), 1);
            // else
            //     _notificationMessages[message] = amount + 1;

            // TODO: Log the message and details.
        }
    }
}
