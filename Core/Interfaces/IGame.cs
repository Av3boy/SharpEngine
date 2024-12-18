using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static Core.Window;

namespace Core.Interfaces;

/// <summary>
///     Contains definitions the Game class must implement.
///     To consider: move into a abstract class so that the game doesn't necesarily have to implement all methods.
/// </summary>
public interface IGame
{
    /// <summary>
    ///     Gets or sets the core settings of the game.
    /// </summary>
    public ISettings CoreSettings { get; }

    /// <summary>
    ///     Gets or sets the camera of the game.
    /// </summary>
    public Camera Camera { get; set; }

    /// <summary>
    ///     Executed when a mouse button is pressed.
    /// </summary>
    /// <param name="e">Contains information about the button press.</param>
    public void HandleMouseDown(MouseButtonEventArgs e);

    /// <summary>
    ///     A method executed when the mouse state is changed
    /// </summary>
    /// <param name="mouse">The state of the mouse on the current frame.</param>
    public void HandleMouse(MouseState mouse);

    /// <summary>
    ///     Gets or sets the event executed when the mouse wheel is scrolled.
    /// </summary>
    /// <param name="direction">The direction the wheel was scrolled.</param>
    /// <param name="e">Information about the mouse scroll event.</param>
    public void HandleMouseWheel(MouseWheelScrollDirection direction, MouseWheelEventArgs e);

    /// <summary>
    ///     Executed when a key is pressed.
    /// </summary>
    /// <param name="input">The state of the keyboard on the current frame.</param>
    /// <param name="deltaTime">Time since the previous frame.</param>
    public void HandleKeyboard(KeyboardState input, float deltaTime);

    /// <summary>
    ///     Executed when the game is first loaded.
    /// </summary>
    public void Initialize();

    /// <summary>
    ///    Executed each frame.
    /// </summary>
    /// <param name="args">Information about the current frame.</param>
    /// <param name="keyboardState">The state of the keyboard this frame.</param>
    /// <param name="mouseState">The state of the mouse this frame.</param>
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState);
}
