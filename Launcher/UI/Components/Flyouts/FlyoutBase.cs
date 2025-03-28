using Microsoft.AspNetCore.Components;

namespace Launcher.UI.Components.Flyouts;

public class FlyoutBase : ComponentBase
{
    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public EventCallback<bool> IsVisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    public virtual async Task OnIsVisibleChangedAsync(bool visibility)
    {
        IsVisible = visibility;
        await IsVisibleChanged.InvokeAsync(visibility);
    }

    public virtual async Task Close()
    {
        await OnClose.InvokeAsync();
        await OnIsVisibleChangedAsync(false);
    }
}
