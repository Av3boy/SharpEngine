using Launcher.Services;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        private List<Project> Projects { get; set; } = [];
        private Project _selectedProject = new();
        private const string _projectsFile = "projects.json";
        private static readonly string[] _sharpProjectFilePickerExtension = [".sharpproject"];

#if DEBUG
        private string _projectsFilePath = Path.Join(Path.GetTempPath(), _projectsFile); 
#else
        private string _projectsFilePath = Path.Join(AppContext.BaseDirectory, _projectsFile);
#endif
        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // TODO: Get current launcher version
            // TODO: Check if GitHub has a new version released.

            var projects = _editorService.LoadProjects(_projectsFilePath);
            
            if (projects is not null)
                Projects.AddRange(projects);
            else
                _notificationService.Show($"Unable to load project file.", false, projects);
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
            {
                Projects.Remove(project);
                // TODO: Delete project files
            }
            else
                _notificationService.Show($"Unable to delete project '{project?.Name}'.", false, _selectedProject, Projects);
        }

        private void ProjectCreated(Project project)
        {
            _editorService.Initialize(project);
            SetProjects(project);
        }

        private async Task OpenExistingFileAsync()
        {
            var pickedFile = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a SharpProject file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, _sharpProjectFilePickerExtension },
                    { DevicePlatform.MacCatalyst, _sharpProjectFilePickerExtension },
                    { DevicePlatform.iOS, _sharpProjectFilePickerExtension },
                    { DevicePlatform.Android, new[] { "application/octet-stream" } }
                })
            });

            if (pickedFile?.FileName.EndsWith(".sharpproject") == false)
            {
                _notificationService.Show("Invalid file type selected. Please select a .sharpproject file.");
                return;
            }

            var project = await _editorService.LoadFileAsync(pickedFile!.FullPath);
            if (project is not null)
                SetProjects(project);
            else
                _notificationService.Show($"Unable to load project file '{pickedFile.FullPath}'");
        }

        private void SetProjects(Project project)
        {
            bool projectAlreadyExists = Projects.Any(p => p.Path == project.Path || p.Name == project.Name);
            if (projectAlreadyExists)
            {
                _notificationService.Show($"Project '{project.Name}' already exists.", false, project);
                return;
            }

            Projects.Add(project);
            string json = JsonSerializer.Serialize(Projects);
            File.WriteAllText(_projectsFilePath, json);
        }
    }
}
