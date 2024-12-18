using Core.Interfaces;

namespace Minecraft;

/// <inheritdoc cref="ISettings" />
public class Settings : ISettings
{
    /// <inheritdoc/>
    public bool UseWireFrame { get; set; }

    /// <inheritdoc/>
    public bool PrintFrameRate { get; set; }
}
