using SharpEngine.Core.Interfaces;
using SharpEngine.Core.Windowing;

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
        var game = new Minimal();

        using var window = new Window(game);
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
    public Minimal()
    {
    }
}
