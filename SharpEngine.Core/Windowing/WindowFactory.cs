using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpEngine.Core.Windowing;

/// <summary>
///     Creates window instances from a collection of <see cref="WindowRegistration"/> entries.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve window dependencies.</param>
/// <param name="registrations">The registrations that describe available windows.</param>
internal sealed class WindowFactory(IServiceProvider serviceProvider, IEnumerable<WindowRegistration> registrations) : IWindowFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IReadOnlyList<WindowRegistration> _registrations = registrations.ToArray();

    /// <summary>
    ///     Gets the names of all registered windows.
    /// </summary>
    public IReadOnlyList<string> RegisteredWindows => [.. _registrations.Select(registration => registration.Name)];

    /// <summary>
    ///     Creates a window by its registration name.
    /// </summary>
    /// <remarks>
    ///     If <paramref name="name"/> is null, the default registration (or the first registration if none are marked default) is used.
    /// </remarks>
    /// <param name="name">Optional name of the window registration to create.</param>
    /// <returns>The created <see cref="Window"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no registrations exist or when a named registration cannot be found.</exception>
    public Window CreateWindow(string? name = null)
    {
        var registration = ResolveRegistration(name);
        var window = registration.Factory(_serviceProvider);
        registration.Configure?.Invoke(_serviceProvider, window);
        return window;
    }

    /// <summary>
    ///     Creates instances for all registered windows in the order they were registered.
    /// </summary>
    /// <returns>A read-only list containing all created window instances.</returns>
    public IReadOnlyList<Window> CreateAllWindows()
        => [.. _registrations.Select(registration => CreateWindow(registration.Name))];

    /// <summary>
    ///     Resolves the appropriate registration for the given name.
    /// </summary>
    /// <param name="name">The optional registration name to resolve.</param>
    /// <returns>
    ///     The matching <see cref="WindowRegistration"/> or returns the default registration when <paramref name="name"/> is null.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when no registrations exist or when a named registration cannot be found.</exception>
    private WindowRegistration ResolveRegistration(string? name)
    {
        if (_registrations.Count == 0)
            throw new InvalidOperationException("No windows have been registered. Call AddWindow during service configuration.");

        if (name is not null)
        {
            var namedRegistration = _registrations.FirstOrDefault(registration => string.Equals(registration.Name, name, StringComparison.OrdinalIgnoreCase));
            return namedRegistration ?? throw new InvalidOperationException($"No window named '{name}' has been registered.");
        }

        return _registrations.FirstOrDefault(registration => registration.IsDefault) ?? _registrations[0];
    }
}
