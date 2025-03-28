using Launcher.Services;
using Launcher.UI.Components;
using Microsoft.AspNetCore.Components;
using SharpEngine.Shared.Dto;
using System.Diagnostics;
using System.Text.Json;

using FilterMode = SharpEngine.Shared.Enums.FilterMode;

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

        private ContextMenu? _contextMenu;

        private List<Project> Projects
        {
            get => _allProjects;
            set
            {
                _allProjects = value;
                _filteredProjects = value;
            }
        }

        private List<Project> _allProjects = [];
        private List<Project> _filteredProjects = [];

        private Project _selectedProject = new();
        private const string _projectsFile = "projects.json";
        private static readonly string[] _sharpProjectFilePickerExtension = [".sharpproject"];

#if !DEBUG
        private readonly string _projectsFilePath = Path.Join(Path.GetTempPath(), _projectsFile);
#else
        private readonly string _projectsFilePath = Path.Join(AppContext.BaseDirectory, _projectsFile);
#endif
        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // TODO: Get current launcher version
            // TODO: Check if GitHub has a new version released.

#if DEBUG
            if (TestData.TestData.SimulateLoading)
            {
                _notificationService.Show("Simulating loading projects.");
                await Task.Delay(10000);
            }

            if (TestData.TestData.LoadTestData)
            {
                _notificationService.Show("Using test data.");
                Projects = [.. TestData.TestData.Projects];
                return;
            }
#endif

            var projects = _editorService.LoadProjects(_projectsFilePath);

            if (projects is not null)
                Projects.AddRange(projects);
            else
                _notificationService.Show($"Unable to load project file.", false, projects);
        }

        private void SearchProjects(string searchValue)
            => _filteredProjects = [.. Projects.Where(p => p.Name!.Contains(searchValue, StringComparison.OrdinalIgnoreCase))];

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
                    { DevicePlatform.Android, [ "application/octet-stream" ] }
                })
            });

            if (pickedFile == null)
                return;

            if (pickedFile.FileName.EndsWith(".sharpproject") == false)
            {
                _notificationService.Show("Invalid file type selected. Please select a .sharpproject file.");
                return;
            }

            var project = await _editorService.LoadFileAsync(pickedFile.FullPath);
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

        private static void OpenInExplorer(string projectPath)
            => Process.Start("explorer.exe", projectPath);

        private void RemoveFromList(Project project)
        {
            var foundProject = Projects.Find(p => p.Id == project.Id);
            if (foundProject is null)
            {
                _notificationService.Show($"Unable to remove project '{project.Name}', project not found.", false, project);
                return;
            }

            Projects.Remove(foundProject);
        }

        private void OnFiltersChanged(Dictionary<string, FilterMode> filters)
        {
            _filteredProjects = [.. Projects.FilterBy(filters)];
            StateHasChanged();
        }
    }
}
