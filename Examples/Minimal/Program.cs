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
        var game = new Minimal(new DefaultSettings()
        {
            UseWireFrame = false,
            WindowOptions = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "Minimal",
            }
        });

        var scene = new Scene();
        using var window = new SharpEngine.Core.Window(game, scene, game.CoreSettings.WindowOptions);
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
