using SharpEngine.Core.Windowing;
using System.Collections.Generic;

namespace DITesting;

/// <summary>
///     Creates configured window instances from DI registrations.
/// </summary>
public interface IWindowFactory
{
    /// <summary>
    ///     Gets the registered window names.
    /// </summary>
    IReadOnlyList<string> RegisteredWindows { get; }

    /// <summary>
    ///     Creates a configured window instance.
    /// </summary>
    /// <param name="name">The optional window registration name.</param>
    /// <returns>A configured <see cref="Window"/>.</returns>
    Window CreateWindow(string? name = null);

    /// <summary>
    ///     Creates one window for each registration.
    /// </summary>
    /// <returns>The configured windows.</returns>
    IReadOnlyList<Window> CreateAllWindows();
}
