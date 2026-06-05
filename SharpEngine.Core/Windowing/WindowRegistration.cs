using System;

namespace SharpEngine.Core.Windowing;

/// <summary>
///     Holds registration information for a window type.
/// </summary>
internal sealed class WindowRegistration
{
    /// <summary>Gets or initializes the name for the window.</summary>
    public required string Name { get; init; }

    /// <summary>Gets or initializes the factory delegate used to create the window instance. </summary>
    /// <remarks>The service provider is supplied to allow resolving dependencies required by the window.</remarks>
    public required Func<IServiceProvider, Window> Factory { get; init; }

    /// <summary>Gets or initializes the optional configuration action invoked immediately after the window is created.</summary>
    public Action<IServiceProvider, Window>? Configure { get; init; }

    /// <summary>Gets or initializes a value indicating whether this registration should be used as the default when no specific window name is requested.</summary>
    public bool IsDefault { get; init; }
}
