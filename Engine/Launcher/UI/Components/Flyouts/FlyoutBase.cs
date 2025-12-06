using Microsoft.AspNetCore.Components;

namespace Launcher.UI.Components.Flyouts;

/// <summary>
///     Represents a base class for flyout components.
/// </summary>
public class FlyoutBase : ComponentBase
{
    /// <summary>Gets or sets whether the flyout is visible.</summary>
    [Parameter]
    public bool IsVisible { get; set; }

    /// <summary>Gets or sets the event executed when the visibility changes.</summary>
    [Parameter]
    public EventCallback<bool> IsVisibleChanged { get; set; }

    /// <summary>Gets or sets the event executed when the flyout is closed.</summary>
    [Parameter]
    public EventCallback OnClose { get; set; }

    /// <summary>
    ///     Handles changes in visibility asynchronously and updates the visibility state.
    /// </summary>
    /// <param name="visibility">Indicates whether the element is currently visible or not.</param>
    /// <returns>
    ///     A <see cref="Task" /> representing an asynchronous operation.
    /// </returns>
    public virtual async Task OnIsVisibleChangedAsync(bool visibility)
    {
        IsVisible = visibility;
        await IsVisibleChanged.InvokeAsync(visibility);
    }

    /// <summary>
    ///     Closes the flyout.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task" /> representing an asynchronous operation.
    /// </returns>
    public virtual async Task Close()
    {
        await OnClose.InvokeAsync();
        await OnIsVisibleChangedAsync(false);
    }
}
