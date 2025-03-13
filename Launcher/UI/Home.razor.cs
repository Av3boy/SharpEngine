using Launcher.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Launcher.UI
{
    /// <summary>Represents the home page of the launcher.</summary>
    public partial class Home : ComponentBase
    {
        [Inject]
        private IEditorService _editorService { get; init; } = default!;

        [Inject]
        private INotificationService _notificationService { get; init; } = default!;

        private bool _showConfirmDelete;
        private bool _showCreateProjectDialog;

        private const string _projectsFile = "projects.json";
        private const string _launcherEnvironmentVariable = "SHARP_ENGINE_LAUNCHER_DIRECTORY";
        private readonly string _currentDirectory = AppContext.BaseDirectory;
        private string _projectFilePath => Path.Join(_currentDirectory, _projectsFile);

        private List<Project> Projects { get; set; } = new();
        //{
        //    // TODO: Remove these mock projects once the actual projects are loaded.
        //    new Project { Name = "Project 1", LastModified = DateTime.Now },
        //    new Project { Name = "Project 2", LastModified = DateTime.Now.AddDays(6).AddHours(2) },
        //};

        private Project _selectedProject = new();

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // TODO: Get current launcher version

            // TODO: Check if GitHub has a new version released.

            CheckEnvironmentVariable();
            LoadProjects();
        }

        private void CheckEnvironmentVariable()
        {
            var launcherDirectory = Environment.GetEnvironmentVariable(_launcherEnvironmentVariable);
            if (string.IsNullOrEmpty(launcherDirectory) || launcherDirectory != _currentDirectory)
                Environment.SetEnvironmentVariable(_launcherEnvironmentVariable, _currentDirectory);
        }

        private void LoadProjects()
        {
            if (!File.Exists(_projectFilePath))
            {
                File.WriteAllText(_projectFilePath, "[]");
                return;
            }

            string json = File.ReadAllText(_projectFilePath);
            var projects = System.Text.Json.JsonSerializer.Deserialize<List<Project>>(json);

            if (projects is not null)
                Projects = projects;
            else
                _notificationService.Show($"Unable to load projects.", false, json, projects);
        }

        private async Task LoadFileAsync(InputFileChangeEventArgs e)
        {
            const string sharpProjectExtension = ".sharpproject";
            string fileName = e.File.Name;
            if (!fileName.EndsWith(sharpProjectExtension, StringComparison.OrdinalIgnoreCase))
            {
                _notificationService.Show($"The file '{fileName}' is not a valid project file.");
                return;
            }

            using var stream = e.File.OpenReadStream();
            using var reader = new StreamReader(stream);
            string fileContent = await reader.ReadToEndAsync();

            var project = System.Text.Json.JsonSerializer.Deserialize<Project>(fileContent);

            if (project is not null)
                Projects.Add(project);
            else
                _notificationService.Show($"Unable to load project file.",  false, project, e.File.Name, fileContent);
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
                _notificationService.Show($"Unable to delete project '{project?.Name}'.", false, _selectedProject, Projects);
        }

        private void ProjectCreated(Project project)
        {
            _editorService.Initialize(project);
            Projects.Add(project);
        }
    }
}
