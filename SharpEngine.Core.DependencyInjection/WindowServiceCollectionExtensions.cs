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

        // We'll always invoke any user-provided configure callback, then wire game handlers.
        handler.Configure((serviceProvider, windowHandler, engine) =>
        {
            // Register a factory that will create and configure the window later on the handler thread.
            windowHandler.AddWindowFactory(() =>
            {
                var window = factory(serviceProvider);

                // Apply optional user configuration and wire up game callbacks for this window.
                configure?.Invoke(serviceProvider, window);

                var game = serviceProvider.GetRequiredService<Game>();

                window.InputManager.OnHandleMouse += game.HandleMouse;
                window.InputManager.OnUpdate += game.Update;
                window.InputManager.OnHandleKeyboard += game.HandleKeyboard;
                window.InputManager.OnButtonMouseDown += game.HandleMouseDown;
                window.InputManager.HandleMouseWheel += game.HandleMouseWheel;
                window.OnAfterRender += game.OnAfterRender;

                if (isDefaultWindow)
                {
                    var initialized = false;
                    var initLock = new object();
                    window.OnLoaded += () =>
                    {
                        lock (initLock)
                        {
                            if (initialized)
                                return;

                            initialized = true;
                        }

                        try
                        {
                            game.Initialize();
                        }
                        catch (Exception ex)
                        {
                            var logger = serviceProvider.GetService<ILogger<Game>>();
                            logger?.LogError(ex, "Error during game initialization.");
                        }
                    };

                    game.Window = window;
                }

                return window;
            }, isDefaultWindow);
        });

        return handler;
    }

    private static void ConfigureWindow(IServiceProvider serviceProvider, Window window)
    {
        var game = serviceProvider.GetRequiredService<Game>();

        window.OnLoaded += game.Initialize;
        window.InputManager.OnHandleMouse += game.HandleMouse;
        window.InputManager.OnUpdate += game.Update;
        window.InputManager.OnHandleKeyboard += game.HandleKeyboard;
        window.InputManager.OnButtonMouseDown += game.HandleMouseDown;
        window.InputManager.HandleMouseWheel += game.HandleMouseWheel;
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
