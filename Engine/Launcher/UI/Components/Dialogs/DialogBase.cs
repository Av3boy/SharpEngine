using Microsoft.AspNetCore.Components;

namespace Launcher.UI.Components
{
    /// <summary>
    ///     Represents the base class for dialogs
    /// </summary>
    public class DialogBase : ComponentBase
    {
        /// <summary>Gets or sets the visibility of the dialog.</summary>
        [Parameter]
        public bool Visible { get; set; }

        /// <summary>Gets or sets the event executed when the dialog visibility is changed.</summary>
        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>Gets or sets the event executed when the dialog is discarded.</summary>
        [Parameter]
        public EventCallback OnDiscardClicked { get; set; }

        /// <summary>
        ///     Discards the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        protected async Task CloseAsync()
        {
            Visible = false;
            await VisibleChanged.InvokeAsync(Visible);
        }
    }
}
