namespace SharpEngine.Core.Windowing;

public static class WindowExtensions
{
    /// <summary>
    ///     Gets a key representing the share group for the window.
    /// </summary>
    /// <param name="window">The window to get the share group key for.</param>
    /// <returns>A key representing the share group for the window.</returns>
    public static object GetShareGroupKey(this Window window)
        => (object?)window.SharedContext ?? (object?)window.GLContext ?? window;
}