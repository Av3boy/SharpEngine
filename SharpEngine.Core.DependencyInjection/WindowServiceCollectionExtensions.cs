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
            var game = serviceProvider.GetRequiredService<Game>();

            game.AttachWindow(window);

            if (isDefaultWindow)
                game.UseWindow(window);

            configure(serviceProvider, window);

            windowHandler.AddWindow(window, isDefaultWindow);
        });

        return handler;
    }

    private static void ConfigureWindow(IServiceProvider serviceProvider, Window window)
    {
        var game = serviceProvider.GetRequiredService<Game>();

        window.OnLoaded += () =>
        {
            game.UseWindow(window);
            game.Initialize();
        };

        window._inputManager.OnHandleMouse += mouse =>
        {
            game.UseWindow(window);
            game.HandleMouse(mouse);
        };

        window._inputManager.OnUpdate += (deltaTime, input) =>
        {
            game.UseWindow(window);
            game.Update(deltaTime, input);
        };

        window._inputManager.OnHandleKeyboard += (input, deltaTime) =>
        {
            game.UseWindow(window);
            game.HandleKeyboard(input, deltaTime);
        };

        window._inputManager.OnButtonMouseDown += (mouse, button) =>
        {
            game.UseWindow(window);
            game.HandleMouseDown(mouse, button);
        };

        window._inputManager.HandleMouseWheel += (direction, scrollWheel) =>
        {
            game.UseWindow(window);
            game.HandleMouseWheel(direction, scrollWheel);
        };

        window.OnAfterRender += frame =>
        {
            game.UseWindow(window);
            game.OnAfterRender(frame);
        };
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
