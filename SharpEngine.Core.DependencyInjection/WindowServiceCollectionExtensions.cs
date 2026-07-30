using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpEngine.Core.Renderers;
using SharpEngine.Core.Scenes;
using SharpEngine.Core.Windowing;
using System;
using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Handlers;

namespace SharpEngine.Core.DependencyInjection;

/// <summary>
///     Provides window-related dependency injection registrations.
/// </summary>
public static class WindowServiceCollectionExtensions
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
    public static HandlerRegistrationBuilder<WindowHandler> AddWindow(
        this HandlerRegistrationBuilder<WindowHandler> handler,
        Func<IServiceProvider, Window>? factory = null,
        Action<IServiceProvider, Window>? configure = null,
        string? name = null,
        bool isDefaultWindow = true)
    {
        ArgumentNullException.ThrowIfNull(handler);

        factory ??= CreateWindow;

        configure ??= ConfigureWindow;

        handler.Configure((serviceProvider, windowHandler, engine) =>
        {
            var window = factory(serviceProvider);
            configure(serviceProvider, window);

            windowHandler.AddWindow(window, isDefaultWindow);
        });

        return handler;
    }

    private static void ConfigureWindow(IServiceProvider serviceProvider, Window window)
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
    }

    private static Window CreateWindow(IServiceProvider serviceProvider)
    {
        var game = serviceProvider.GetRequiredService<Game>();
        var scene = serviceProvider.GetRequiredService<Scene>();
        var windowLogger = serviceProvider.GetRequiredService<ILogger<Window>>();
        var renderers = serviceProvider.GetServices<RendererBase>();

        var window = new Window(game.Camera, scene, game.Camera.Settings, windowLogger, renderers);

        return window;
    }
}
