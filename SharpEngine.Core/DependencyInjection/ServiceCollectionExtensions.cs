using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpEngine.Core.Windowing;
using System;
using System.Linq;

namespace SharpEngine.Core.DependencyInjection;

/// <summary>
///     Provides window-related dependency injection registrations.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers a window factory and an optional configuration callback.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Creates the window instance.</param>
    /// <param name="configure">Configures the created window.</param>
    /// <param name="name">The optional registration name.</param>
    /// <param name="isDefault">Whether this registration should be used as the default app window.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddWindow(
        this IServiceCollection services,
        Func<IServiceProvider, Window> factory,
        Action<IServiceProvider, Window>? configure = null,
        string? name = null,
        bool isDefault = false)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(factory);

        services.TryAddSingleton<IWindowFactory, WindowFactory>();

        var registrationName = name ?? $"window-{services.Count(service => service.ServiceType == typeof(WindowRegistration))}";
        services.AddSingleton(new WindowRegistration
        {
            Name = registrationName,
            Factory = factory,
            Configure = configure,
            IsDefault = isDefault
        });

        return services;
    }
}
