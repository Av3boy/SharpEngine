using SharpEngine.Core.Interfaces;
using Silk.NET.Windowing;

namespace SharpEngine.Core.Entities.Views.Settings;

/// <summary>
///     Contains definitions for settings to control view behaviour.
/// </summary>
public interface IViewSettings : ISettings
{
    /// <summary>Gets or sets the sensitivity of the mouse in the view.</summary>
    public float MouseSensitivity { get; set; }

    /// <summary>Gets or sets the options for the window.</summary>
    /// <remarks>Recommended default is <see cref="WindowOptions.Default"/>.</remarks>
    public WindowOptions WindowOptions { get; set; }
}
