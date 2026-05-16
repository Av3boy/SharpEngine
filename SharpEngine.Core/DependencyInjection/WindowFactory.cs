using SharpEngine.Core.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DITesting;

internal sealed class WindowRegistration
{
    public required string Name { get; init; }

    public required Func<IServiceProvider, Window> Factory { get; init; }

    public Action<IServiceProvider, Window>? Configure { get; init; }

    public bool IsDefault { get; init; }
}

internal sealed class WindowFactory(IServiceProvider serviceProvider, IEnumerable<WindowRegistration> registrations) : IWindowFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IReadOnlyList<WindowRegistration> _registrations = registrations.ToArray();

    public IReadOnlyList<string> RegisteredWindows => _registrations.Select(registration => registration.Name).ToArray();

    public Window CreateWindow(string? name = null)
    {
        var registration = ResolveRegistration(name);
        var window = registration.Factory(_serviceProvider);
        registration.Configure?.Invoke(_serviceProvider, window);
        return window;
    }

    public IReadOnlyList<Window> CreateAllWindows()
        => _registrations.Select(registration => CreateWindow(registration.Name)).ToArray();

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
