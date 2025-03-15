using Microsoft.AspNetCore.Components;
using Launcher.Services;
using CommunityToolkit.Maui.Storage;

namespace Launcher.UI.Components.Dialogs
{
    public partial class CreateProjectDialog : DialogBase
    {
        [Inject]
        private INotificationService _notificationService { get; init; } = default!;

        private string _createProjectDialogVisibleClass => Visible ? "create-project-dialog-visible" : "create-project-dialog-hidden";
        private Project _newProject = new();

        /// <summary>Gets or sets the event executed when the project is created.</summary>
        [Parameter]
        public EventCallback<Project> OnProjectCreated { get; set; }

        private async Task OnCreateProjectSubmitClickedAsync()
        {
            await OnProjectCreated.InvokeAsync(_newProject);

            _newProject = new();
            await CloseAsync();
        }

        private async Task HandleFileSelection()
        {
            var result = await FolderPicker.Default.PickAsync();

            if (result is not null && result.IsSuccessful)
                _newProject.Path = result.Folder.Path;
            else
                _notificationService.Show("No file selected.", false);
        }
    }
}
