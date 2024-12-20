﻿using Core;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Minecraft;

/// <summary>
///     Represents the entry point of the application.
/// </summary>
public static class Program
{
    private static void Main()
    {
        Settings gameSettings = new()
        {
            UseWireFrame = false,
        };

        Scene scene = new Scene();
        Game game = new Game(scene, gameSettings);

        var nativeWindowSettings = new NativeWindowSettings()
        {
            // TODO: Change through settings
            ClientSize = new Vector2i(800, 600),
            Title = "Minecraft",

            // This is needed to run on macos
            Flags = ContextFlags.ForwardCompatible,
        };

        using var window = new Window(game, scene, GameWindowSettings.Default, nativeWindowSettings);
        window.Run();
    }
}
