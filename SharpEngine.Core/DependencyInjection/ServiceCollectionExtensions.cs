using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Windowing;
using System;
using System.Linq;
using SharpEngine.Core.Interfaces;

namespace SharpEngine.Core.DependencyInjection;

/// <summary>
///     Provides window-related dependency injection registrations.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers a window factory and an optional configuration callback.
    /// </summary>
    /// <remarks>
    ///     When creating multiple windows the last one created will act as the default window.
    ///     To specify a different default window, try tweaking the <paramref name="isDefaultWindow"/> parameter.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Creates the window instance.</param>
    /// <param name="configure">Configures the created window.</param>
    /// <param name="name">The optional registration name.</param>
    /// <param name="isDefaultWindow">Whether this registration should be used as the default app window.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddWindow(
        this IServiceCollection services,
        Func<IServiceProvider, Window>? factory = null,
        Action<IServiceProvider, Window>? configure = null,
        string? name = null,
        bool isDefaultWindow = true)
    {
        ArgumentNullException.ThrowIfNull(services);

        factory ??= serviceProvider =>
        {
            var game = serviceProvider.GetRequiredService<Game>();
            var scene = serviceProvider.GetRequiredService<Scene>();
            var windowLogger = serviceProvider.GetRequiredService<ILogger<Window>>();
            var renderers = serviceProvider.GetServices<RendererBase>();

            var window = new Window(game.Camera, scene, game.Camera.Settings, windowLogger, renderers);
            if (!string.IsNullOrWhiteSpace(name))
                window.Title = name;

            return window;
        };

        if (configure == null)
        {
            configure = (serviceProvider, window) =>
            {
                var game = serviceProvider.GetRequiredService<Game>();

                window.OnLoaded += game.Initialize;
                window._inputManager.OnHandleMouse += game.HandleMouse;
                window._inputManager.OnUpdate += game.Update;
                window._inputManager.OnHandleKeyboard += game.HandleKeyboard;
                window._inputManager.OnButtonMouseDown += game.HandleMouseDown;
                window._inputManager.HandleMouseWheel += game.HandleMouseWheel;
                window.OnAfterRender += game.OnAfterRender;

                game.Window = window;
            };
        }

        services.TryAddSingleton<IWindowFactory, WindowFactory>();

        var registrationName = name ?? $"window-{services.Count(service => service.ServiceType == typeof(WindowRegistration))}";
        services.AddSingleton(new WindowRegistration
        {
            Name = registrationName,
            Factory = factory,
            Configure = configure,
            IsDefault = isDefaultWindow
        });

        return services;
    }
}
