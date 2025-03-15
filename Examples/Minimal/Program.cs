using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Scenes;

using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Minimal;

/// <summary>
///     Represents the entry point of the application.
/// </summary>
public static class Program
{
    /// <summary>
    ///     The entry point of the application.
    /// </summary>
    public static void Main(string[] _)
    {
        var game = new Minimal(new DefaultSettings());
        var scene = new Scene();

        using var window = new SharpEngine.Core.Window(game.Camera, scene, game.Camera.Settings);
        window.Run();
    }
}

/// <summary>
///     Represents the game instance.
/// </summary>
public class Minimal : Game
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Minimal" />
    /// </summary>
    /// <param name="settings">Provides configuration options for the instance.</param>
    public Minimal(ISettings settings)
    {
        CoreSettings = settings;
    }
}
